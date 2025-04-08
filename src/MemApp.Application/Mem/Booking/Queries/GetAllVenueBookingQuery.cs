using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Booking.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetAllVenueBookingQuery : IRequest<VenueBookingListVm>
    {
       public VenueBookingSearchReq Model { get; set; }= new VenueBookingSearchReq();
    }
    public class GetAllVenueBookingQueryHandler : IRequestHandler<GetAllVenueBookingQuery, VenueBookingListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllVenueBookingQueryHandler(IMemDbContext context,IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;

        }

        public async Task<VenueBookingListVm> Handle(GetAllVenueBookingQuery request, CancellationToken cancellationToken)
        {
            var result = new VenueBookingListVm();
            if (!await _permissionHandler.HasRolePermissionAsync(3603))
            {
                result.HasError = true;
                result.Messages?.Add("You have no permission to view");
                return result;
            }

            try
            {
                var pdata = await _context.VenueBookings.Include(i => i.VenueBookingDetails.Where(q => q.IsActive && q.SaleStatus != "Cancel"))
                  .Where(q => q.IsActive && q.CreatedOn.AddDays(2) <= DateTime.Now && q.PaymentAmount == 0 && q.BookingStatus == "Pending" && q.IsCancelled == false

                  )
                  .ToListAsync(cancellationToken);

                if (pdata.Any())
                {
                    pdata.ForEach(q => q.IsActive = false);
                    pdata.ForEach(q => q.BookingStatus = "Cancel");

                    pdata.ForEach(q => q.VenueBookingDetails.All(q => q.IsActive = false));
                    pdata.ForEach(q => q.VenueBookingDetails.All(q => q.IsCancelled = true));


                    await _context.SaveChangesAsync(cancellationToken);
                }


                if (request.Model.BookingStatus== "Cancel")
                {
                    var canceldata = await _context.VenueBookings
                   .Include(i => i.VenueBookingDetails.Where(q => q.SaleStatus == "Cancel"))
                    .ThenInclude(i => i.VenueAddOnsItemDetails)
                   .Where(q => q.IsActive && q.BookingStatus == "Cancel"
                    && (!(request.Model.StartDate == null || request.Model.StartDate == "") ? q.BookedDate >= DateTime.Parse(request.Model.StartDate) : true)
                   && (!(request.Model.EndDate == null || request.Model.EndDate == "") ? q.BookedDate <= DateTime.Parse(request.Model.EndDate) : true)
                   && (request.Model.TicketCriteriaId > 0 ? q.BookingCriteriaId == request.Model.TicketCriteriaId : true)
                   && (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MemberShipNo.Contains(request.Model.SearchText) : true)

                   ).OrderByDescending(o => o.Id)
                   .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);


                    if(canceldata.TotalCount > 0)
                    {
                        var availabilityCanceldata = await _context.AvailabilityDetails.Where(q => q.IsActive).ToListAsync(cancellationToken);
                        var memberCancelList = await _context.RegisterMembers
                      .Where(q => q.IsActive && q.IsMasterMember == true)
                      .Select(s => new { s.Id, s.FullName })
                      .ToListAsync(cancellationToken);
                   

                    result.HasError = false;
                    result.DataCount = canceldata.TotalCount;
                    result.DataList = canceldata.Data.Select(s => new VenueBookingReq
                    {
                        Id = s.Id,
                        BookedNo = s.BookedNo,
                        BookedDate = s?.BookedDate == null ? "" : s.BookedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        CreatedDate = s.CreatedOn.AddHours(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BookingStatus = s.BookingStatus,
                        MemberShipNo = s.MemberShipNo,
                        MemberId = s.MemberId,
                        MemberName = memberCancelList?.FirstOrDefault(q => q.Id == s.MemberId)?.FullName,
                        Amount = s.Amount,
                        VatAmount = s.VatAmount,
                        ServiceAmount = s.ServiceAmount,
                        TotalAmount = s.TotalAmount,
                        PaymentAmount = s.PaymentAmount,
                        DiscountAmount = s.DiscountAmount,
                        PaymentDate = s?.PaymentDate == null ? "" : s.PaymentDate.AddHours(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BookingCriteriaId = s.BookingCriteriaId,
                        BookingCriteria = s.BookingCriteria,
                        BookingPrice = s.BookingPrice,
                        OrderFrom = s.OrderFrom,
                        Note= s.Note,
                        AdditionalFee= s.AdditionalFee,

                        VenueBookingDetailReqs = s.VenueBookingDetails.Select(s1 => new VenueBookingDetailReq
                        {
                            Id = s1.Id,
                            IsCancelled = s1.IsCancelled,
                            VenueId = s1.VenueId,
                            VenueTitle = s1.VenueTitle,
                            BookingDate = s1?.BookingDate == null ? "" : s1.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            AvailabilityId = s1.AvailabilityId,
                            ServiceChargeAmount = s1.ServiceChargeAmount ?? 0,
                            VatAmount = s1.VatAmount ?? 0,
                            AvailabilityTitle = availabilityCanceldata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.Title,
                            StartTime = availabilityCanceldata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.StartTime,
                            EndTime = availabilityCanceldata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.EndTime,
                            CancelNote= s1.CancellationNote ?? "",
                            VenueBookingAddOnsItemReqs = s1.VenueAddOnsItemDetails?.Select(addon => new VenueBookingAddOnsItemReq
                            {
                                Id = addon.Id,
                                AddOnsItemId = addon.AddOnsItemId,
                                BookingDetailId = Convert.ToInt32(addon.BookingDetailId),
                                Title = addon.Title,
                                Description = addon.Description,
                                Price = addon.Price,
                                PriceDate = addon.PriceDate,
                                
                            }).ToList()
                        }).ToList()

                    }).ToList();

                    }


                    return result;
                }        
              
                var data = await _context.VenueBookings
                    .Include(i => i.VenueBookingDetails.Where(q => q.IsActive && q.SaleStatus!="Cancel" && !q.IsCancelled) )
             .ThenInclude(i => i.VenueAddOnsItemDetails)
                    .Where(q => q.IsActive && q.BookingStatus !="Cancel"
                     && (!(request.Model.StartDate == null || request.Model.StartDate=="") ? q.BookedDate >= DateTime.Parse(request.Model.StartDate) : true)
                    && (!(request.Model.EndDate == null || request.Model.EndDate=="") ? q.BookedDate <= DateTime.Parse(request.Model.EndDate) : true)
                    && ( request.Model.TicketCriteriaId>0 ? q.BookingCriteriaId == request.Model.TicketCriteriaId : true)
                    && (!string.IsNullOrEmpty(request.Model.SearchText) ? q.MemberShipNo.Contains(request.Model.SearchText) : true)

                    ).OrderByDescending(o=>o.Id)
                    .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);


                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    var availabilitydata = await _context.AvailabilityDetails.Where(q => q.IsActive).ToListAsync(cancellationToken);
                    var memberList = await _context.RegisterMembers
                        .Where(q => q.IsActive && q.IsMasterMember==true)
                        .Select(s=> new {s.Id, s.FullName})
                        .ToListAsync(cancellationToken);



                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select( s => new VenueBookingReq
                    {
                        Id = s.Id,
                        BookedNo = s.BookedNo,
                        BookedDate = s?.BookedDate == null ? "" : s.BookedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        CreatedDate=s.CreatedOn.AddHours(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BookingStatus = s.BookingStatus,
                        MemberShipNo = s.MemberShipNo,                     
                        MemberId = s.MemberId,
                        MemberName= memberList?.FirstOrDefault(q=> q.Id== s.MemberId)?.FullName,
                        Amount = s.Amount,
                        VatAmount=s.VatAmount,
                        ServiceAmount=s.ServiceAmount,
                        TotalAmount=s.TotalAmount,
                        PaymentAmount=s.PaymentAmount,
                        DiscountAmount=s.DiscountAmount,
                        PaymentDate= s?.PaymentDate == null ? "" : s.PaymentDate.AddHours(-6).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BookingCriteriaId = s.BookingCriteriaId,
                        BookingCriteria = s.BookingCriteria,
                        BookingPrice = s.BookingPrice,
                        OrderFrom= s.OrderFrom,
                        AdditionalFee = s.AdditionalFee,
                        VenueBookingDetailReqs = s.VenueBookingDetails.Select(s1=> new VenueBookingDetailReq
                        {
                            Id=s1.Id,
                            IsCancelled = s1.IsCancelled,
                            VenueId = s1.VenueId,
                            VenueTitle= s1.VenueTitle,                           
                            BookingDate = s1?.BookingDate == null ? "" : s1.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            AvailabilityId= s1.AvailabilityId,
                            ServiceChargeAmount = s1.ServiceChargeAmount??0,
                            VatAmount = s1.VatAmount??0,
                            AvailabilityTitle =  availabilitydata?.FirstOrDefault(q1=>q1.Id== s1.AvailabilityId)?.Title,
                            StartTime = availabilitydata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.StartTime,
                            EndTime = availabilitydata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.EndTime,
                            VenueBookingAddOnsItemReqs = s1.VenueAddOnsItemDetails?.Select(addon => new VenueBookingAddOnsItemReq
                            {
                            Id = addon.Id,
                                AddOnsItemId = addon.AddOnsItemId,
                                BookingDetailId = Convert.ToInt32(addon.BookingDetailId),
                                Title = addon.Title,
                                Description = addon.Description,
                                Price = addon.Price,
                                PriceDate = addon.PriceDate
                            }).ToList()
                        }).ToList()

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
