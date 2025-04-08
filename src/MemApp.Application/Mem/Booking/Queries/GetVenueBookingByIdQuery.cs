using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AddOnsItems.Models;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetVenueBookingByIdQuery : IRequest<VenueBookingVm>
    {
        public int Id { get; set; }
    }


    public class GetVenueBookingByIdQueryHandler : IRequestHandler<GetVenueBookingByIdQuery, VenueBookingVm>
    {
        private readonly IMemDbContext _context;
        public GetVenueBookingByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<VenueBookingVm> Handle(GetVenueBookingByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = new VenueBookingVm();
                var data = await _context.VenueBookings
                    .Include(i => i.Member)
                   
                    .Include(i => i.VenueBookingDetails.Where(q => q.IsActive))
                    .ThenInclude(i => i.Availabilities)
                    .Include(i => i.VenueBookingDetails.Where(q => q.IsActive))
                    .ThenInclude(i => i.VenueAddOnsItemDetails)
                    .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);
                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;

                    result.Data = new VenueBookingReq
                    {
                        Id = data.Id,
                        BookedNo = data.BookedNo,
                        BookedDate = data.BookedDate.ToString("dd-MMM-yyyy"),
                        BookingStatus = data.BookingStatus,
                        MemberShipNo = data.MemberShipNo,
                        MemberId = data.MemberId,
                        Amount = data.Amount,
                        PaymentAmount = data.PaymentAmount,
                        PaymentDate = data.PaymentDate.ToString("yyyy-MM-dd"),
                        BookingCriteriaId = data.BookingCriteriaId,
                        BookingCriteria = data.BookingCriteria,
                        BookingPrice = data.BookingPrice,
                        OrderFrom = data.OrderFrom,
                        TotalAmount = data.TotalAmount,
                        ServiceAmount = data.ServiceAmount,
                        VatAmount = data.VatAmount,
                        MemberName  = data.Member.FullName,
                        DiscountAmount = data.DiscountAmount,
                        RefPhoneNo = data.RefPhoneNo,
                        RefRelation = data.RefRelation,
                        BookingPurpose = data.BookingPurpose,
                        RefName = data.RefName,



                        VenueBookingDetailReqs = data.VenueBookingDetails.Select(s1 => new VenueBookingDetailReq
                        {
                            Id = s1.Id,
                            VenueId = s1.VenueId,
                            VenueTitle = s1.VenueTitle,
                            BookingDate = s1.BookingDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            AvailabilityId = s1.AvailabilityId,  // Removed ?? 0 if AvailabilityId is not nullable
                            VenueBookingAddOnsItemReqs = s1.VenueAddOnsItemDetails.Select(addon => new VenueBookingAddOnsItemReq
                            {
                                AddOnsItemId = addon.AddOnsItemId,
                                Title = addon.Title,
                                Price = addon.Price
                            }).ToList()
                        }).ToList()

                    //VenueBookingAddOnsItemReqs = data.VenueAddOnsItemDetails?.Select(addon=> new VenueBookingAddOnsItemReq
                    //{
                    //    Id= addon.Id,
                    //    AddOnsItemId = addon.AddOnsItemId,
                    //    BookingId = addon.BookingId,
                    //    Title = addon.Title,
                    //    Description = addon.Description,
                    //    Price = addon.Price,
                    //    PriceDate = addon.PriceDate,
                    //}
                    //).ToList()


                };

                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
