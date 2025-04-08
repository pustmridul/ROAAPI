using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Sales.SaleTicket.Model;


namespace MemApp.Application.Mem.Sales.SaleTicket.Queries
{
    public class GetAllSaleTicketQuery : IRequest<MemSaleTicketListVm>
    {
      public  SaleTicketSearchReq Model { get; set; }= new SaleTicketSearchReq();
    }

    public class GetAllSaleTicketQueryHandler : IRequestHandler<GetAllSaleTicketQuery, MemSaleTicketListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllSaleTicketQueryHandler(IMemDbContext context)
        {
            _context = context;    
        }

        public async Task<MemSaleTicketListVm> Handle(GetAllSaleTicketQuery request, CancellationToken cancellationToken)
        {
            var result = new MemSaleTicketListVm();
            try
            {

                var data = await _context.SaleMasters.Where(q => q.IsActive )
                .ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);
                result.DataList = data.Data.Select(s => new MemSaleTicketRes() { MemServiceId = s.MemServiceId }).ToList();

               
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

