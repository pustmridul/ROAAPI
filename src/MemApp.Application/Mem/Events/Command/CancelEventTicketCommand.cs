using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Application.Mem.Booking.Queries;
using MemApp.Application.Mem.Events.Models;
using MemApp.Application.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MemApp.Application.Mem.Events.Command
{
    public class CancelEventTicketCommand : IRequest<bool>
    {
        public SaleEventTicketReq Model { get; set; } = new SaleEventTicketReq();
    }

    public class CancelEventTicketCommandHandler : IRequestHandler<CancelEventTicketCommand, bool>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        ICurrentUserService _currentUserService;
        IMemLedgerService _memLedgerService;


        public CancelEventTicketCommandHandler(
            IMemDbContext context,
            IMediator mediator,
            ICurrentUserService currentUserService,
            IMemLedgerService memLedgerService
            )
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _memLedgerService = memLedgerService;
        }
        public async Task<bool> Handle(CancelEventTicketCommand request, CancellationToken cancellation)
        {

            
            
            var eventTicket = await _context.SaleEventTickets
                .Include(c=>c.SaleEventTicketDetails)
                .Where(c=>c.Id==request.Model.Id).FirstOrDefaultAsync(cancellation);

            if (eventTicket != null)
            {
                var selectedDetailIdsToCancel = request.Model.SaleEventTicketDetailReqs.Select(c=>c.Id).ToList();

                foreach (var ticketDetail in eventTicket.SaleEventTicketDetails.Where(c => selectedDetailIdsToCancel.Contains(c.Id)))
                {
                    // Assign a value to the specific property (SomeProperty) of each element
                    ticketDetail.SaleStatus = "Cancel";
                    ticketDetail.CancellationNote = request.Model.SaleEventTicketDetailReqs.Where(c => c.Id == ticketDetail.Id).FirstOrDefault()?.Note;
                    ticketDetail.RefundAmount = request.Model.SaleEventTicketDetailReqs.Where(c => c.Id == ticketDetail.Id).FirstOrDefault()?.TicketPrice;
                }
                await _context.SaveChangesAsync(cancellation);
                if (eventTicket.SaleEventTicketDetails.All(c => c.SaleStatus?.Trim() == "Cancel"))
                {
                    eventTicket.SaleStatus = "Cancel";
                    await _context.SaveChangesAsync(cancellation);
                }
                var note = "";
                //Create ledger
                foreach (var item in request.Model.SaleEventTicketDetailReqs)
                {
                    if (!string.IsNullOrEmpty(item.Note))
                    {
                        note += item.Note+". ";
                    }
                }
                string preFix = "R";
                var refundNo = "";
                var max = _context.MemLedgers.Where(q => q.RefundId.StartsWith(preFix))
                    .Select(s => s.RefundId.Replace(preFix, "")).DefaultIfEmpty().Max();

                if (string.IsNullOrEmpty(max))
                {
                    refundNo = preFix + "0000001";
                }
                else
                {
                    refundNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000000");
                }

                var memLedger = new MemLedgerVm()
                {
                    TransactionFrom= _currentUserService.AppId,
                    ReferenceId= eventTicket.InvoiceNo,
                    TransactionType= "EVENT",
                    Amount = request.Model.SaleEventTicketDetailReqs.Sum(c => c.TicketPrice),
                    Dates = DateTime.Now.Date,
                    PrvCusID = request.Model.MemberId.ToString(),
                    TOPUPID = null,
                    RefundId = refundNo,
                    UpdateBy = _currentUserService.Username,
                    UpdateDate = DateTime.Now,
                    DatesTime = DateTime.Now,
                    Notes = "Event Ticket Price Amount : " + request.Model.Amount + ", Vat Amount: " + request.Model.VatAmount + ", Service Charge Amount : " + request.Model.ServiceAmount + "Refund Amount: " + request.Model.RefundAmount,
                    PayType = "",
                    Description = "Event Ticket: " + request.Model.InvoiceNo + ". " + "Sale date: " + request.Model.InvoiceDate + ". " + "Refund Amount: " + request.Model.SaleEventTicketDetailReqs.Sum(c => c.TicketPrice)+ ". "+ (string.IsNullOrEmpty(note) ? "" : ". Cancellation Note: " + note),
                };

                var result = await _memLedgerService.CreateMemLedger(memLedger);

                return result;
            }
            return false;

            
        }
    }
}
