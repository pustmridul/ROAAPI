using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscSales.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MiscSales.Queries
{
    public class GetMiscSaleByIdQuery : IRequest<MiscSaleVm>
    {
        public int Id { get; set; }
    }

    public class GetMiscSaleByIdQueryHandler : IRequestHandler<GetMiscSaleByIdQuery, MiscSaleVm>
    {
        private readonly IMemDbContext _context;
        public GetMiscSaleByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MiscSaleVm> Handle(GetMiscSaleByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MiscSaleVm();
            var data = await _context.MiscSales
                .Include(i=>i.RegisterMember)
                .Include(i=>i.MiscSaleDetails).ThenInclude(i=>i.MiscItem)
                .SingleOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MiscSaleReq
                {
                    Id = data.Id, 
                    InvoiceDate = data.InvoiceDate,
                    InvoiceNo = data.InvoiceNo,
                    MemberId = data.MemberId,
                    MemberShipNo=data.RegisterMember?.MembershipNo,
                    MemberText=data.RegisterMember?.FullName,
                    Note=data.Note,
                    MiscSaleDetailReqs= data.MiscSaleDetails.Select(s=>new MiscSaleDetailReq
                    {
                        Id=s.Id,
                        MiscSaleId=s.Id,
                        ItemId=s.ItemId,
                        ItemText=s.MiscItem.Name,
                        Quantity=s.Quantity,
                        UnitPrice=s.UnitPrice,

                    }).ToList()

                };
            }

            return result;
        }
    }
}
