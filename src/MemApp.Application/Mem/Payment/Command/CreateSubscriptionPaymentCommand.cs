using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Payment.Model;
using MemApp.Application.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Payment.Command
{

    public class CreateSubscriptionPaymentCommand : IRequest<SubscriptionPayDetailListVm>
    {
        public SubscriptionPaymentReq Model { get; set; }
    }

    public class CreateSubscriptionPaymentCommandHandler : IRequestHandler<CreateSubscriptionPaymentCommand, SubscriptionPayDetailListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IMemLedgerService _memLedgerService;
        public CreateSubscriptionPaymentCommandHandler(IMemDbContext context, IMediator mediator, IMemLedgerService memLedgerService)
        {
            _context = context;
            _mediator = mediator;
            _memLedgerService = memLedgerService;
        }
        public async Task<SubscriptionPayDetailListVm> Handle(CreateSubscriptionPaymentCommand request, CancellationToken cancellation)
        {
            var result = new SubscriptionPayDetailListVm();
            try
            {
                if (request.Model.SubscriptionPayDetails.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Subscription  Fee not added");
                    return result;
                }   
                var obj = await _context.SubscriptionPayments
                 
                    .SingleOrDefaultAsync(q=>q.RegisterMemberId== request.Model.RegisterMemberId 
                    && q.PaymentDate== request.Model.PaymentDate);
               
                if(obj == null)
                {
                    obj = new SubscriptionPayment
                    {
                        MemberPayment = request.Model.PaymentAmount,
                        RegisterMemberId = request.Model.RegisterMemberId,
                        MemberShipNo = request.Model.MemberShipNo,
                        PaymentDate = request.Model.PaymentDate
                    };
                    _context.SubscriptionPayments.Add(obj);
                }
                                                            
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    List<SubscriptionPaymentDetail> objList = new List<SubscriptionPaymentDetail>();
                    var listofPayment = await _context.SubscriptionPaymentDetails
                        .Where(q => q.RegisterMemberId == obj.RegisterMemberId)
                        .ToListAsync(cancellation);

                    foreach (var item in request.Model.SubscriptionPayDetails)
                    {
                        var objd = listofPayment.SingleOrDefault(q => q.RegisterMemberId == obj.RegisterMemberId && q.SubscriptionFeesId == item.SubscriptionFeesId);

                        if (objd == null)
                        {
                            SubscriptionPaymentDetail data = new SubscriptionPaymentDetail
                            {
                                SubscriptionPaymentId = obj.Id,
                                SubscriptionFeesId = item.SubscriptionFeesId,
                                RegisterMemberId = obj.RegisterMemberId,
                                PaymentFeesDate = obj.PaymentDate,
                                PaymentFees= item.PaymentFees
                            };
                            objList.Add(data);
                        }
                     }
                     _context.SubscriptionPaymentDetails.AddRange(objList);
                    if( await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        List<MemLedgerVm> memLedgers = new List<MemLedgerVm>();
                        foreach(var d in obj.SubscriptionPaymentDetails)
                        {
                            var memObj = new MemLedgerVm
                            {
                                PrvCusID = obj.RegisterMemberId.ToString(),
                                Amount =-1* d.PaymentFees,
                                Description ="",
                                Dates = DateTime.Now,
                                TOPUPID = d.Id.ToString(),
                                DatesTime = DateTime.Now,
                                PayType = "Subscription",
                                BankCreditCardName = "",
                                ChequeCardNo = "",
                                Notes = "",
                                ServiceChargeAmount = 0,
                                RefundId = "",
                                TransactionType="SUBSCRIPTION",
                                TransactionFrom=obj.MemberShipNo,
                            };
                            memLedgers.Add(memObj);
                        }
                        await _memLedgerService.CreateBulkMemLedger(memLedgers);
                    }
                  
                }
            }
            catch (Exception ex)
            {                   
                result.HasError = true;
                result.Messages.Add("Error "+ ex.Message.ToString());
            }
            return result;
        }
    }
}
