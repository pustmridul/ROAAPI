using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Sales.SaleTicket.Model;


namespace MemApp.Application.Mem.Sales.SaleTicket.Queries
{
    public class GetAllSaleTicketByMemberShipNoQuery : IRequest<MemSaleTicketListVm>
    {
        public string MemberShipNo { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllSaleTicketByMemberShipNoQueryHandler : IRequestHandler<GetAllSaleTicketByMemberShipNoQuery, MemSaleTicketListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllSaleTicketByMemberShipNoQueryHandler(IMemDbContext context)
        {
            _context = context;    
        }

        public async Task<MemSaleTicketListVm> Handle(GetAllSaleTicketByMemberShipNoQuery request, CancellationToken cancellationToken)
        {
            var result = new MemSaleTicketListVm();
            try
            {
         
                var data = await _context.SaleMasters.Where(q => q.IsActive && q.MemberShipNo== request.MemberShipNo)
                        .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new MemSaleTicketRes
                    {
                        Id = s.Id,
                        InvoiceDate = s.InvoiceDate,
                        InvoiceNo = s.InvoiceNo
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

