using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Sales.SaleTicket.Model;
using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.SaleTicket.Command
{
    public class CreateSaleTicketCommand : IRequest<MemSaleTicketVm>
    {
        public MemSaleTicketReq Model { get; set; } = new MemSaleTicketReq();
    }

    public class CreateSaleTicketCommandHandler : IRequestHandler<CreateSaleTicketCommand, MemSaleTicketVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public CreateSaleTicketCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
        }
        public async Task<MemSaleTicketVm> Handle(CreateSaleTicketCommand request, CancellationToken cancellation)
        {
            bool newObj = false;
            var result = new MemSaleTicketVm();
          
            try
            {
                var obj = await _context.SaleMasters
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);

                if(obj == null)
                { 
                    newObj = true;

                    obj = new SaleMaster()
                    {
                        MemberId = request.Model.MemberId,
                        InvoiceDate = request.Model.InvoiceDate,
                        MemberShipNo = request.Model.MembershipNo,
                        IsActive = true
                    };
                    _context.SaleMasters.Add(obj);
                }

                if (newObj)
                {
                    string preFix = "S" + request.Model.InvoiceDate.ToString("yyMMdd");

                    var max = _context.SaleMasters.Where(q => q.InvoiceNo.StartsWith(preFix))
                        .Select(s => s.InvoiceNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                    if (string.IsNullOrEmpty(max))
                    {
                        obj.InvoiceNo = preFix + "0001";
                    }
                    else
                    {
                        obj.InvoiceNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                    }



                }
                obj.InvoiceStatus = request.Model.InvoiceStatus;
                obj.SaleType = request.Model.SaleType;
                obj.ServiceTypeId = request.Model.ServiceTypeId;
                obj.ServiceTicketId = request.Model.ServiceTicketId;
                obj.MemServiceId= request.Model.MemServiceId;
                obj.ExpenseAmount = request.Model.ExpenseAmount??0;
                obj.ServiceChargeAmount= request.Model.ServiceChargeAmount ?? 0;
                obj.ServiceChargePercent= request.Model.ServiceChargePercent ?? 0;
                obj.VatChargePercent = request.Model.VatChargePercent ?? 0;
                obj.VatChargeAmount= request.Model.VatChargeAmount ?? 0;
                obj.ReservationDate = request.Model.ReservationDate ?? null;
                obj.OrderFrom= request.Model.OrderFrom;
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    if (newObj)
                    {
                        foreach (var std in request.Model.SaleTicketDetailReqs)
                        {
                            var objDetail = new SaleTicketDetail()
                            {
                                SaleMasterId = obj.Id,
                                Quantity = std.Quantity,
                                UnitPrice = std.UnitPrice,
                                UnitName= std.UnitName,
                                ServiceTicketDetailId = std.ServiceTickerDetailId
                            };
                            obj.ExpenseAmount += objDetail.Quantity * objDetail.UnitPrice;
                            _context.SaleTicketDetails.Add(objDetail);
                        }






                        foreach (var std in request.Model.SaleLayoutDetailReqs)
                        {
                            var objDetail = new SaleLayoutDetail()
                            {
                                SaleMasterId = obj.Id,
                                AreaLayoutId = std.AreaLayoutId,
                                AreaLayoutTitle = std.AreaLayoutTitle,
                                TableId = std.TableId,
                                TableName = std.TableName,
                                NoofChair=std.NoofChair
                            };
                            _context.SaleLayoutDetails.Add(objDetail);
                        }
                    }
                    else
                    {

                        var toBeDelete = new List<SaleTicketDetail>();
                        foreach (var od in obj.SaleTicketDetails)
                        {
                            var has = request.Model.SaleTicketDetailReqs.Any(q => q.Id == od.Id);
                            if (!has)
                            {
                                toBeDelete.Add(od);
                            }
                        }
                        var toBeLayDelete = new List<SaleLayoutDetail>();
                        foreach (var od in obj.SaleLayoutDetails)
                        {
                            var has = request.Model.SaleLayoutDetailReqs.Any(q => q.Id == od.Id);
                            if (!has)
                            {
                                toBeLayDelete.Add(od);
                            }
                        }
                        _context.SaleLayoutDetails.RemoveRange(toBeLayDelete);
                        _context.SaleTicketDetails.RemoveRange(toBeDelete);


                    }
                    await _context.SaveChangesAsync(cancellation);
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("something wrong");
                }

              

            }
            catch (Exception ex)
            {
               
                result.HasError = true;
                result.Messages.Add("Exception" + ex.Message);
            }
            return result;
        }
    }
}
