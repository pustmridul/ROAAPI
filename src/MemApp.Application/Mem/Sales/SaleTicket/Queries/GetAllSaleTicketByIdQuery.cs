using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Sales.SaleTicket.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.SaleTicket.Queries
{
    public class GetSaleTicketByIdQuery : IRequest<MemSaleTicketVm>
    {
        public int Id { get; set; }
    }


    public class GetSaleTicketByIdQueryHandler : IRequestHandler<GetSaleTicketByIdQuery, MemSaleTicketVm>
    {
        private readonly IMemDbContext _context;
        public GetSaleTicketByIdQueryHandler(IMemDbContext context)
        {
            _context = context;    
        }

        public async Task<MemSaleTicketVm> Handle(GetSaleTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MemSaleTicketVm();
            try
            {

                    var data = await _context.SaleMasters
                        .Include(i=>i.SaleTicketDetails)
                        .Include(i=>i.SaleLayoutDetails)
                        .SingleOrDefaultAsync(q => q.IsActive && q.Id == request.Id, cancellationToken);
                  

                if (data== null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data.InvoiceDate = data.InvoiceDate;
                    result.Data.InvoiceNo = data.InvoiceNo;
                    result.Data.ExpenseAmount= data.ExpenseAmount;
                    result.Data.VatChargePercent = data.VatChargePercent;
                    result.Data.VatChargeAmount = data.VatChargeAmount;
                    result.Data.ServiceChargeAmount = data.ServiceChargeAmount;
                    result.Data.ServiceChargePercent= data.ServiceChargePercent;
                    result.Data.MemberId= data.MemberId;
                    result.Data.MembershipNo = data.MemberShipNo;
                    result.Data.ReservationDate = data.ReservationDate;
                    result.Data.SaleType = data.SaleType;
                    result.Data.InvoiceStatus = data.InvoiceStatus;
                    result.Data.OrderFrom = data.OrderFrom;
                    result.Data.ServiceTicketId = data.ServiceTicketId;


                    result.Data.SaleTicketDetailReqs= data.SaleTicketDetails.Select(s => new SaleTicketDetailReq
                    {
                        Id = s.Id,
                        ServiceTickerDetailId= s.ServiceTicketDetailId,
                        Quantity= s.Quantity,
                        UnitName = s.UnitName,
                        UnitPrice= s.UnitPrice,
                  
                    }).ToList();

                    result.Data.SaleLayoutDetailReqs = data.SaleLayoutDetails.Select(s => new SaleLayoutDetailReq
                    {
                        Id = s.Id,
                        TableId = s.TableId,
                        TableName = s.TableName,
                        AreaLayoutId = s.AreaLayoutId,
                        AreaLayoutTitle = s.AreaLayoutTitle,
                        NoofChair=s.NoofChair

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

