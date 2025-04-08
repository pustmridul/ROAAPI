using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Services;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetAllBookedVenueByMemberQuery : IRequest<VenueBookingListVm>
    {
       public int MemberId { get; set; }
    }


    public class GetAllBookedVenueByMemberQueryHandler : IRequestHandler<GetAllBookedVenueByMemberQuery, VenueBookingListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        
        public GetAllBookedVenueByMemberQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler )
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<VenueBookingListVm> Handle(GetAllBookedVenueByMemberQuery request, CancellationToken cancellationToken)
        {
            var result = new VenueBookingListVm();

            try
            {
                
                if (await _permissionHandler.IsTempMember())
                {
                    result.HasError = true;
                    result.Messages.Add("You have no permission to access, please contact with authority.");
                }          
                var pdata = await _context.VenueBookings.Include(i => i.VenueBookingDetails.Where(q => q.IsActive))
                    .Where(q => q.IsActive && q.BookedDate.AddDays(2) <= DateTime.Now && q.PaymentAmount == 0 && q.BookingStatus=="Pending"
                    && q.IsCancelled==false
                    && q.MemberId==request.MemberId
                    )
                    .ToListAsync(cancellationToken);

                if(pdata.Any() )
                {
                    pdata.ForEach(q => q.IsActive = false);
                    pdata.ForEach(q => q.BookingStatus = "Cancel");
                    pdata.ForEach(q=>q.VenueBookingDetails.All(q => q.IsActive = false));

                    await _context.SaveChangesAsync(cancellationToken);
                }
               
                var data = await _context.VenueBookings
                    .Include(i => i.VenueBookingDetails.Where(q => q.IsActive))
                    .Where(q => q.IsActive && q.BookingStatus !="Cancel" && q.MemberId== request.MemberId                   
                    ).OrderByDescending(o=> o.Id).ThenByDescending(o=>o.BookingStatus)
                    .ToListAsync(cancellationToken);


                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    var availabilitydata = await _context.AvailabilityDetails.Where(q => q.IsActive).ToListAsync(cancellationToken);
                    var memberList = await _context.RegisterMembers
                        .Where(q => q.IsActive && q.IsMasterMember==true)
                        .Select(s=> new {s.Id, s.FullName})
                        .ToListAsync(cancellationToken);


                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select( s => new VenueBookingReq
                    {
                        Id = s.Id,
                        BookedNo = s.BookedNo,
                        BookedDate = s?.CreatedOn == null ? "" : s.CreatedOn.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
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
                        PaymentDate= s?.PaymentDate == null ? "" : s.PaymentDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BookingCriteriaId = s.BookingCriteriaId,
                        BookingCriteria = s.BookingCriteria,
                        BookingPrice = s.BookingPrice,
                        OrderFrom= s.OrderFrom,
                        VenueBookingDetailReqs = s.VenueBookingDetails.Select(s1=> new VenueBookingDetailReq
                        {
                            Id=s1.Id,
                            IsCancelled = s1.IsCancelled,
                            VenueId = s1.VenueId,
                            VenueTitle= s1.VenueTitle,                           
                            BookingDate = s1?.BookingDate == null ? "" : s1.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            AvailabilityId= s1.AvailabilityId,
                            AvailabilityTitle=  availabilitydata?.FirstOrDefault(q1=>q1.Id== s1.AvailabilityId)?.Title,
                            StartTime = availabilitydata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.StartTime,
                            EndTime = availabilitydata?.FirstOrDefault(q1 => q1.Id == s1.AvailabilityId)?.EndTime

                        }).ToList()

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
