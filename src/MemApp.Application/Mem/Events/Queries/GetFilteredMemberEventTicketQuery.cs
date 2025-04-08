using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Queries;
using MemApp.Application.Mem.Events.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Events.Queries
{
    public class GetFilteredMemberEventTicketQuery : IRequest<EventTicketListVm>
    {
        public int MemberId { get; set; }
        public int EventId { get; set; }
    }

    public class GetFilteredMemberEventTicketQueryHandler : IRequestHandler<GetFilteredMemberEventTicketQuery, EventTicketListVm>
    {
        private readonly IMemDbContext _context;
        public GetFilteredMemberEventTicketQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<EventTicketListVm> Handle(GetFilteredMemberEventTicketQuery request, CancellationToken cancellationToken)
        {
            var result = new EventTicketListVm();
            var SaleEventTicketDetails = await _context.SaleEventTicketDetails
                .Include(c => c.SaleEventTicket) // Include the related SaleEventTicket
                .Where(c => c.EventId == request.EventId && c.SaleEventTicket.MemberId == request.MemberId) // Add the MemberId condition
                .ToListAsync();

            var serviceTickets = await _context.ServiceTickets.ToListAsync(cancellationToken);

            if (!SaleEventTicketDetails.Any() || SaleEventTicketDetails.Count()<1)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                var memberObj = await _context.RegisterMembers.Select(s => new { s.Id, s.MembershipNo, s.CardNo, s.FullName })
                      .FirstOrDefaultAsync(q => q.Id == request.MemberId, cancellationToken);
                result.HasError = false;

                foreach (var item in SaleEventTicketDetails)
                {
                    var sTicket = serviceTickets.FirstOrDefault(s => s.Id == item.EventId);

                    var evt = new EventTictetReq()
                    {
                        Location = sTicket != null ? sTicket.Location : "",
                        EventTitle = item.EventTitle ?? "",
                        MemberShipNo = memberObj.MembershipNo ?? "",
                        MemberName = memberObj?.FullName ?? "",
                        EventDate = sTicket != null ? sTicket.EventDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : "",
                        InvoiceDate = item.SaleEventTicket.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        InvoiceNo = item.SaleEventTicket.InvoiceNo,
                        EventTokens = string.IsNullOrEmpty(item.EventTokens) ? new List<string>() : item.EventTokens.Split(',').ToList(),
                        TicketCodeNo = item.TicketCodeNo ?? "",
                        TicketPrice = item.TicketPrice ?? 0,
                        TicketCriteria = item.TicketCriteria,
                        TableTitle = item.TableTitle ?? "",
                        TicketText = item.TicketText ?? "",
                        AreaLayoutTitle = item.AreaLayoutTitle ?? "",
                        ChairNo = item.NoofChair.ToString() ?? "0",

                    };
                    result.DataList.Add(evt);
                }


            }

            return result;
        }
    }
}