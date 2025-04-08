using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.ServiceSales.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.ServiceSales.Queries
{
    public class GetAllServiceBuyByMemberQuery : IRequest<ServiceSaleListVm>
    {
        public int MemberId { get; set; }
    }


    public class GetAllServiceBuyMemberQueryHandler : IRequestHandler<GetAllServiceBuyByMemberQuery, ServiceSaleListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;

        public GetAllServiceBuyMemberQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ServiceSaleListVm> Handle(GetAllServiceBuyByMemberQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceSaleListVm();

            try
            {

                if (await _permissionHandler.IsTempMember())
                {
                    result.HasError = true;
                    result.Messages.Add("You have no permission to access, please contact with authority.");
                }

                var data = await _context.ServiceSales
                 .Include(i => i.RegisterMember)
                 .Include(i => i.ServiceSaleDetails.Where(q => q.IsActive && q.SaleStatus != "Cancel"))
                 .Where(q => q.IsActive
                 && q.SaleStatus != "Cancel"
                 && q.MemberId == request.MemberId
                 ).OrderByDescending(o => o.Id)
                     .ToListAsync(cancellationToken);



                if (data.Count == 0)
                {
                    result.HasError = false;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new ServiceSaleReq
                    {
                        Id = s.Id,
                        InvoiceDate = s.InvoiceDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsActive = s.IsActive,
                        MemberId = s.MemberId,
                        InvoiceNo = s.InvoiceNo,
                        MembershipNo = s.MembershipNo,
                        OrderFrom = s.OrderFrom,
                        TotalAmount = s.TotalAmount,
                        Amount = s.Amount,
                        VatAmount = s.VatAmount,
                        ServiceSaleDetailReqs = s.ServiceSaleDetails.Select(s => new ServiceSaleDetailReq
                        {
                            Id = s.Id,
                            IsActive = s.IsActive,
                            Quantity = s.Quantity,
                            UnitName = s.UnitName,
                            UnitPrice = s.UnitPrice,
                            ServiceChargePercent = s.ServiceChargePercent,
                            MemServiceId = s.MemServiceId,
                            ServiceCriteriaId = s.ServiceCriteriaId,
                            ServiceCriteriaText = s.ServiceCriteriaText,
                            ServicePrice = s.UnitPrice ?? 0,
                            SaleDay = s.RevDay,
                            ServiceQty = s.Quantity ?? 0,
                            SaleMonth = s.RevMonth,
                            SaleYear = s.RevYear,
                            ServiceSaleId = s.ServiceSaleId,
                            VatChargeAmount = s.VatChargeAmount,
                            ServiceChargeAmount = s.ServiceChargeAmount

                        }).ToList(),

                    }).ToList();

                }
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
