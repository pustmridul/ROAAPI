using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Events.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Events.Queries
{
    public class GetSaleEventTicketByIdQuery : IRequest<SaleEventTicketVm>
    {
        public int Id { get; set; }
    }


    public class GetSaleEventTicketByIdQueryHandler : IRequestHandler<GetSaleEventTicketByIdQuery, SaleEventTicketVm>
    {
        private readonly IMemDbContext _context;
        public GetSaleEventTicketByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<SaleEventTicketVm> Handle(GetSaleEventTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new SaleEventTicketVm();
            var data = await _context.SaleEventTickets
                .Include(i=>i.SaleEventTicketDetails.Where(q=>q.IsActive))
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new SaleEventTicketReq
                {
                    Id = data.Id,
                    InvoiceNo = data.InvoiceNo,
                    InvoiceDate = data.InvoiceDate,     
                    SaleStatus = data.SaleStatus,
                    MemberShipNo = data.MemberShipNo,
                    MemberId = data.MemberId,
                    Amount = data.Amount,
                    PaymentAmount = data.PaymentAmount,
                    PaymentDate = data.PaymentDate,
                  
                    OrderFrom = data.OrderFrom,
                    SaleEventTicketDetailReqs = data.SaleEventTicketDetails.Select(s1 => new SaleEventTicketDetailReq
                    {
                        Id = s1.Id,
                        EventId = s1.EventId,
                        EventTitle = s1.EventTitle,                    
                        EventTokens=s1.EventTokens,
                        AreaLayoutId=s1.AreaLayoutId ??0,
                        AreaLayoutTitle=s1.AreaLayoutTitle,
                        TableId=s1.TableId,
                        TableTitle=s1.TableTitle,
                        NoofChair=s1.NoofChair,
                        TicketCriteriaId = s1.TicketCriteriaId,
                        TicketCriteria = s1.TicketCriteria,
                        TicketPrice = s1.TicketPrice,
                        TicketText = s1.TicketText,
                    }).ToList()

                };
               
            }

            return result;
        }
    }
}
