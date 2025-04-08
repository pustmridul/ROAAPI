using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.ServiceSales.Models;
using MemApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;
using Microsoft.AspNetCore.Http;

namespace MemApp.Application.Mem.ServiceSales.Queries
{
    public class GetServiceSaleTicketListByIdQuery : IRequest<ServiceSaleTicketReq>
    {
        public int ServiceSaleId { get; set; }
    }


    public class GetServiceSaleTicketListByIdQueryHandler : IRequestHandler<GetServiceSaleTicketListByIdQuery, ServiceSaleTicketReq>
    {
        private readonly IMemDbContext _context;     

        public GetServiceSaleTicketListByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceSaleTicketReq> Handle(GetServiceSaleTicketListByIdQuery request, CancellationToken cancellationToken)
        {
            ServiceSaleTicketReq result = new ServiceSaleTicketReq();
            try
            {
                var data = await _context.ServiceSales
                    .Include(i => i.RegisterMember)
                    .Include(i => i.ServiceSaleDetails.Where(q => q.IsActive))              
                    .SingleOrDefaultAsync(q => q.IsActive && q.Id == request.ServiceSaleId, cancellationToken);                                      

                if (data != null)
                {
                    //var serviceTicket= await _context.MemServices
                    //    .FirstOrDefaultAsync(q => q.Id == data.ServiceTicketId ??0, cancellationToken);
                    var serviceTicket = await _context.MemServices
                       .Where(q => q.ServiceTypeId ==  7)
                       .AsNoTracking()
                       .ToListAsync(cancellationToken);
                   
                    
                  //  result.TicketTitle = serviceTicket!=null? serviceTicket.Title : "";
                    result.InvoiceDate = data.InvoiceDate.ToString("yyyy-MM-dd");
                    result.InvoiceNo = data.InvoiceNo;
                   
                    result.Quantity = data.ServiceSaleDetails.Sum(s=>s.Quantity) ?? 0;
                    result.MemberShipNo = data.RegisterMember?.MembershipNo ?? "";
                    result.MemberName = data.RegisterMember?.FullName??"";
                    result.TotalAmount = data.TotalAmount;
                    result.CreatedByName = data.CreatedByName;


                    if (data.ServiceSaleDetails != null)
                    {
                        foreach (var item in data.ServiceSaleDetails)
                        {
                            var objD = new ServiceSaleTicketDetailReq()
                            {
                                TicketCodeNo = item.TicketCodeNo ?? "",
                                TicketDate = item.RevDate.ToString("yyyy-MM-dd"),
                                TicketQuantity = item.Quantity ?? 0,
                                TicketPrice=Math.Round( item.UnitPrice ?? 0,2),
                                ServiceText= serviceTicket.SingleOrDefault(q => q.Id == item.MemServiceId)?.Title ?? "",
                                TicketType = item.UnitName??"",
                                VatAmount= Math.Round( item.VatChargeAmount,2),
                                ServiceCharge=Math.Round( item.ServiceChargeAmount,2)
                                
                            };
                            result.TicketDetails.Add(objD);
                        }
                    }                   
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
