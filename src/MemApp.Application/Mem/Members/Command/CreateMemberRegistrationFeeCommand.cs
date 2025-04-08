using AutoMapper;
using AutoMapper.Execution;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Application.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberRegistrationFeeCommand : IRequest<RegisterMemberVm>
    {
        public MemberFeeReq Model { get; set; } = new MemberFeeReq();
    }

    public class CreateMemberRegistrationFeeCommandHandler : IRequestHandler<CreateMemberRegistrationFeeCommand, RegisterMemberVm>
    {
        private readonly IMediator _mediator;
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMemLedgerService _ledgerService;
        private readonly IBroadcastHandler _broadcastHandler;

        public CreateMemberRegistrationFeeCommandHandler(IMediator mediator,
            IMemDbContext context,
            ICurrentUserService currentUserService,
            IMemLedgerService ledgerService,
            IBroadcastHandler broadcastHandler)
        {
            _mediator = mediator;
            _context = context;
            _currentUserService = currentUserService;
            _ledgerService = ledgerService;
            _broadcastHandler = broadcastHandler;
        }
        public async Task<RegisterMemberVm> Handle(CreateMemberRegistrationFeeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new RegisterMemberVm();
            List<string> registrationFees = new List<string>();
            decimal totalPayment = 0;
            List<MemLedgerVm> memLedgers = new List<MemLedgerVm>();
            try
            {
                var obj =await _context.RegisterMembers
                    .SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);
                if(obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Member Not Found");
                }
                else
                {
                    var objList= await _context.MemberFeesMaps.Where(q=>q.RegisterMemberId== request.Model.MemberId).ToListAsync(cancellation);
                    var toBeFeeDelete = new List<MemberFeesMap>();
                    foreach (var od in objList)
                    {
                        var has = request.Model.MemberFeesMapReqs.Any(q => q.Id == od.Id);
                        if (!has)
                        {
                            toBeFeeDelete.Add(od);
                            var memObj = new MemLedgerVm
                            {
                                PrvCusID = obj.Id.ToString(),
                                Amount =od.Amount,
                                Description = "Registration Remove Fees for" + od.MemberFeesTitle + ", Date :" + od.PaymentDate.ToString(),
                                Dates = DateTime.Now,
                                TOPUPID = "",
                                DatesTime = DateTime.Now,
                                PayType = "RegistrationFees",
                                BankCreditCardName = "",
                                ChequeCardNo = "",
                                Notes = "Registration Remove Fees for" + od.MemberFeesTitle + ", Date :" + od.PaymentDate.ToString(),
                                ServiceChargeAmount = 0,
                                RefundId = ""

                            };
                            await _ledgerService.CreateMemLedger(memObj);
                        }
                    }
                    
                    _context.MemberFeesMaps.RemoveRange(toBeFeeDelete);
                

                    foreach (var f in request.Model.MemberFeesMapReqs)
                    {                           
                        var exObj = objList.SingleOrDefault(q => q.Id == f.Id);
                        if(exObj == null)
                        {
                            var fee = new MemberFeesMap
                            {
                                RegisterMemberId = request.Model.MemberId,
                                MemberShipFeeId = f.MemberShipFeeId,
                                MemberFeesTitle = f.MemberFeesTitle,
                                Amount = f.Amount,
                                PaymentDate = f.PaymentDate,
                                IsActive=true
                            };
                            _context.MemberFeesMaps.Add(fee);
                        }
                        else
                        {
                            exObj.MemberFeesTitle = f.MemberFeesTitle;
                            exObj.Amount = f.Amount;
                            exObj.PaymentDate = DateTime.Now;                             
                        }                       
                        var memObj = new MemLedgerVm
                        {
                            PrvCusID = obj.Id.ToString(),
                            Amount = -1 * f.Amount,
                            Description = "Registration Fees for" + f.MemberFeesTitle + ", Date :" + f.PaymentDate.ToString(),
                            Dates = DateTime.Now,
                            TOPUPID = "",
                            DatesTime = DateTime.Now,
                            PayType = "RegistrationFees",
                            BankCreditCardName = "",
                            ChequeCardNo = "",
                            Notes = "Registration Fees for" + f.MemberFeesTitle + ", Date :" + f.PaymentDate.ToString(),
                            ServiceChargeAmount = 0,
                            RefundId = ""

                        };
                        memLedgers.Add(memObj);
                        
                        registrationFees.Add(f.MemberFeesTitle);

                        totalPayment = totalPayment + f.Amount;
                        
                    }         
                    if(await _context.SaveChangesAsync(cancellation)>0)
                    {
                        result.HasError= false;
                        result.Messages.Add("Save Success");                    
                        await _ledgerService.CreateBulkMemLedger(memLedgers);
                        string message = "";
                        string subject = "";
                        if (obj.Phone != null)
                        {

                            message =
                                $" Subscription : {string.Join(",", registrationFees)}" +
                                $" Amount: {Math.Round(totalPayment, 2)}";
                            await _broadcastHandler.SendSms(obj.Phone, message, "English");
                            message = "";
                        }
                        if (obj.Email != null)
                        {

                            message = "Dear Sir, MemberShipNo : " + obj.MembershipNo + ", Your Registration Fees amount : " + Math.Round(totalPayment, 2) + ",Fees for : " + string.Join(",",registrationFees);
                            subject = "Registration Fee (Cadet College Club Ltd) ";
                            await _broadcastHandler.SendEmail(obj.Email, subject, message);
                            message = "";
                            subject = "";
                        }


                    }
                }
            }catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" +ex.ToString());

            }

            return await _mediator.Send(new GetMemberByIdQuery()
            {
                Id = request.Model.MemberId
            });        
        }
    }
}
