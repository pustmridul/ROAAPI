using AutoMapper.Execution;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.MiscSales.Models;
using MemApp.Application.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities.Sale;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MemApp.Application.Mem.MiscSales.Command
{
    public class CreateMiscSaleCommand : IRequest<MiscSaleVm>
    {
        public MiscSaleReq Model { get; set; } = new MiscSaleReq();
    }

    public class CreateMiscSaleCommandHandler : IRequestHandler<CreateMiscSaleCommand, MiscSaleVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IMemLedgerService _memLedgerService;
        public CreateMiscSaleCommandHandler(IMemLedgerService memLedgerService,IMemDbContext context, IMediator mediator,  ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _memLedgerService = memLedgerService;
        }
        public async Task<MiscSaleVm> Handle(CreateMiscSaleCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new MiscSaleVm();
            try
            {
                bool newObj = false;
                var obj = await _context.MiscSales
                    .Include(i=>i.RegisterMember)
                    .Include(i => i.MiscSaleDetails).ThenInclude(i=>i.MiscItem)
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    newObj = true;
                    obj = new MiscSale()
                    {
                    };
                    _context.MiscSales.Add(obj);
                }
                if (newObj)
                {
                    string preFix = "M" + request.Model.InvoiceDate.ToString("yyMMdd");

                    var max = _context.MiscSales.Where(q => q.InvoiceNo.StartsWith(preFix))
                        .Select(s => s.InvoiceNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                    if (string.IsNullOrEmpty(max))
                    {
                        obj.InvoiceNo = preFix + "0001";
                    }
                    else
                    {
                        obj.InvoiceNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                    }
                    obj.InvoiceDate = request.Model.InvoiceDate;
                }

                obj.MemberId = request.Model.MemberId;               
                obj.Note = request.Model.Note;


                await _context.SaveChangesAsync(cancellation);

                foreach (var d in request.Model.MiscSaleDetailReqs)
                {
                    var objDetail = await _context.MiscSaleDetails
                        .SingleOrDefaultAsync(q => q.Id == d.Id && q.ItemId == d.ItemId && q.MiscSaleId == obj.Id, cancellation);

                    if (objDetail == null)
                    {
                        objDetail = new MiscSaleDetail()
                        {
                            MiscSaleId = obj.Id
                        };
                        _context.MiscSaleDetails.Add(objDetail);
                    }
                    objDetail.ItemId = d.ItemId;
                    objDetail.Quantity = d.Quantity;
                    objDetail.UnitPrice = d.UnitPrice;               
                }
                var toBeDeleteObjs = new List<MiscSaleDetail>();

                foreach (var d1 in obj.MiscSaleDetails)
                {
                    var has = request.Model.MiscSaleDetailReqs.Any(q => q.Id == d1.Id);
                    if (!has)
                    {
                        toBeDeleteObjs.Add(d1);
                    }
                }


                if (toBeDeleteObjs.Count > 0)
                {
                    _context.MiscSaleDetails.RemoveRange(toBeDeleteObjs);
                }
                

                if (await _context.SaveChangesAsync(cancellation)>0)
                {
                    var memLedger = new MemLedgerVm()
                    {
                        ReferenceId= obj.InvoiceNo,
                        Amount = (-1) * obj.MiscSaleDetails.Sum(c=>c.Quantity* c.UnitPrice),
                        Dates = obj.InvoiceDate,
                        PrvCusID = obj.MemberId.ToString(),
                        TOPUPID = "",
                        UpdateBy = _currentUserService.Username,
                        UpdateDate = DateTime.Now,
                        DatesTime = DateTime.Now,
                        Notes = "MiscsSell :  SellId : " + obj.Id,
                        PayType = "",
                        TransactionType = "MISCSALE",
                        TransactionFrom = _currentUserService.AppId,
                        Description = "MemberShip No : " + obj?.RegisterMember?.MembershipNo + ", Card No : " +  obj?.RegisterMember?.CardNo + ",Invoice NO : " + obj.InvoiceNo + "Invoice Date :" + obj.InvoiceDate ,
                    };

                    await _memLedgerService.CreateMemLedger(memLedger);
                }


                result.HasError = false;
                result.Data.Id = obj.Id;
                result.Data.InvoiceNo = obj.InvoiceNo;
                result.Data.InvoiceDate = obj.InvoiceDate;
                result.Messages.Add("Save Success");
                return result;
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" + ex.Message);
                return result;
            }
           
        }
    }

}
