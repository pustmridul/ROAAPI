using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Mem.ServiceSales.Models;

namespace MemApp.Application.Mem.Sales.Command
{
    public class CancelServiceSaleCommand : IRequest<bool>
    {
          public ServiceSaleReq Model { get; set; } = new ServiceSaleReq();
    }

    public class CancelServiceSaleCommandHandler : IRequestHandler<CancelServiceSaleCommand, bool>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        ICurrentUserService _currentUserService;
        IMemLedgerService _memLedgerService;


        public CancelServiceSaleCommandHandler(
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
        public async Task<bool> Handle(CancelServiceSaleCommand request, CancellationToken cancellation)
        {
            try
            {
                decimal refundAmount = 0;
                var serviceSale = await _context.ServiceSales.Include(c => c.ServiceSaleDetails).Where(c => c.Id == request.Model.Id).FirstOrDefaultAsync();
                if (serviceSale != null)
                {
                    var selectedDetailIdsToCancel = request.Model.ServiceSaleDetailReqs.Select(c => c.Id).ToList();


                    foreach (var ticketDetail in serviceSale.ServiceSaleDetails.Where(c => selectedDetailIdsToCancel.Contains(c.Id)))
                    {
                        ticketDetail.SaleStatus = "Cancel";
                        ticketDetail.CancellationNote = request.Model.ServiceSaleDetailReqs.Where(c => c.Id == ticketDetail.Id).FirstOrDefault()?.Note;
                        ticketDetail.RefundAmount = request.Model.ServiceSaleDetailReqs.Where(c => c.Id == ticketDetail.Id).FirstOrDefault()?.SubTotal ?? 0;
                        refundAmount += ticketDetail.RefundAmount;
                    }
                    await _context.SaveChangesAsync(cancellation);
                    if (serviceSale.ServiceSaleDetails.All(c => c.SaleStatus?.Trim() == "Cancel"))
                    {
                        serviceSale.SaleStatus = "Cancel";
                    }
                    var note = "";
                    //Create ledger
                    foreach (var item in request.Model.ServiceSaleDetailReqs)
                    {
                        if (!string.IsNullOrEmpty(item.Note))
                        {
                            note += item.Note + ". ";
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
                        Amount = refundAmount,
                        ReferenceId= serviceSale.InvoiceNo,
                        TransactionFrom = _currentUserService.AppId,
                        TransactionType = "SERVICE",//_currentUserService.AppId
                        Dates = DateTime.Now.Date,
                        PrvCusID = request.Model.MemberId.ToString() ?? "",
                        TOPUPID = null,
                        RefundId = refundNo,
                        UpdateBy = _currentUserService.Username,
                        UpdateDate = DateTime.Now,
                        DatesTime = DateTime.Now,
                        //   Notes = "Event Ticket Price Amount : " + request.Model.Amount + ", Vat Amount: " + request.Model.VatAmount + ", Service Charge Amount : " + request.Model.ServicePrice + "Refund Amount: " + request.Model.ServiceSaleDetailReqs.Sum(c => c.UnitPrice),
                        PayType = "",
                        Description = "Service Ticket: " + request.Model.InvoiceNo + ". " + "Sale date: " + request.Model.InvoiceDate + ". " + "Refund Amount: " + request.Model.ServiceSaleDetailReqs.Sum(c => c.SubTotal) + ". " + (string.IsNullOrEmpty(note) ? "" : ". Cancellation Note: " + note),
                    };

                    var result = await _memLedgerService.CreateMemLedger(memLedger);

                    return result;
                }
                return false;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
           
           


        }
    }
}
