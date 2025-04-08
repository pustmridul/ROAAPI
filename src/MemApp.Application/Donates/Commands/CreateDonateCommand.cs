using Hangfire;
using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Command;
using MemApp.Application.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donates.Commands;

public class CreateDonateCommand : IRequest<Result>
{
    public DonateDto Model { get; set; } = new DonateDto();
}

public class CreateDonateCommandHandler : IRequestHandler<CreateDonateCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IUserLogService _userLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBackgroundJobClientV2 _backgroundJobClient;
    private readonly IMemLedgerService _memLedgerService;
    public CreateDonateCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, 
        ICurrentUserService currentUserService, IBackgroundJobClientV2 backgroundJobClientV2, IMemLedgerService memLedgerService)
    {
        _context = context;
        _mediator = mediator;
        _userLogService = userLogService;
        _currentUserService = currentUserService;
        _backgroundJobClient = backgroundJobClientV2;
        _memLedgerService = memLedgerService;
    }
    public async Task<Result> Handle(CreateDonateCommand request, CancellationToken cancellation)
    {
        try
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var result = new Result();
            var obj = new Donate();
            if (request.Model.topup is not null)
            {
                await _mediator.Send(new CreateTopUpCommand() { Model = request.Model.topup });
            }

            string preFix = "D" + DateTime.Now.ToString("yyMMdd");

                var max = _context.Donates.Where(q => q.DonateNo.StartsWith(preFix))
                    .Select(s => s.DonateNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                if (string.IsNullOrEmpty(max))
                {
                    obj.DonateNo = preFix + "0001";
                }
                else
                {
                    obj.DonateNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                }

            
            obj.MemberId = request.Model.MemberId;
            obj.DonationId = request.Model.DonationId;
            obj.DonateDate = DateTime.Now;
            obj.Amount = request.Model.Amount;
            obj.Note = request.Model.Note;

            _context.Donates.Add(obj);
            await _context.SaveChangesAsync(cancellation);
            var memberObj = await _context.RegisterMembers.Select(s => new
            {
                s.Id,
                s.MembershipNo,
                s.CardNo,
                s.MemberId,
                s.Phone,
                s.Email
            }).FirstOrDefaultAsync(q => q.Id == obj.MemberId, cancellation);


            var donation = await _context.Donations.SingleOrDefaultAsync(q => q.Id == request.Model.DonationId, cancellation);

            var memLedger = new MemLedgerVm()
            {
                ReferenceId = obj.DonateNo,
                Amount = (-1) * request.Model.Amount,
                Dates = obj?.DonateDate,
                PrvCusID = obj?.MemberId.ToString() ?? "",
                TOPUPID = "",
                UpdateBy = _currentUserService.Username,
                UpdateDate = DateTime.Now,
                DatesTime = DateTime.Now,
                Notes = "Donate :  DonateId : " + obj?.Id,
                PayType = "",
                TransactionFrom = _currentUserService.AppId,
                TransactionType = "VENUEBOOKING",
                Description = "MemberShip No : " + memberObj?.MembershipNo + ", Card No : " + memberObj?.CardNo + ",Donation : " + donation?.Title + "," +
                " Donate No : " + obj.DonateNo + "Date :" + obj.DonateDate + ", Amount : " + obj.Amount 
            };

            await _memLedgerService.CreateMemLedger(memLedger);


            if (memberObj != null)
            {
                var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                string message = "";
                string subject = "";
                if (memberObj.Phone != null)
                {
                   message = "Dear CCCL Member # " + memberObj.MembershipNo + " Tk. " + Math.Round(obj.Amount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendSms(memberObj.Phone, message, "English", null, null));
                    message = "";
                }
                if (memberObj.Email != null)
                {
                   message = "Dear CCCL Member # " + memberObj.MembershipNo + " Tk. " + Math.Round(obj.Amount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";

                    subject = "Donate (Cadet College Club Ltd) ";
                    _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.SendEmail(memberObj.Email, subject, message,null,null));
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}
