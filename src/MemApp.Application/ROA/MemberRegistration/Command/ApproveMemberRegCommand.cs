using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using ResApp.Application.ROA.RoaSubcription;
using ResApp.Application.ROA.RoaSubcription.Command;


namespace ResApp.Application.ROA.MemberRegistration.Command
{
    public class ApproveMemberRegCommand : IRequest<Result<MemberRegistrationInfoDto>>
    {
        // public string? UserName {  get; set; }
        public int? MemberId { get; set; }
        public int? MemberCategoryId { get; set; }
        public decimal? MembershipFee { get; set; }
        public decimal? SubscriptionFee { get; set; }
        public string? Note { get; set; }
        public DateTime? SubscriptionStarts { get; set; }
        //  public bool IsApproved { get; set; }
    }

    public class ApproveMemberRegCommandHandler : IRequestHandler<ApproveMemberRegCommand, Result<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public ApproveMemberRegCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<MemberRegistrationInfoDto>> Handle(ApproveMemberRegCommand request, CancellationToken cancellationToken)
        {
            var checkAdmin = _currentUserService.Current().UserName;

            var result = new Result<MemberRegistrationInfoDto>();
            if (checkAdmin != "Super Admin")
            {
                result.HasError = true;
                result.Messages.Add("Invalid request!!!");
                return result;
            }
            var checkMemberExist = await _context.MemberRegistrationInfos
               .FirstOrDefaultAsync(q => q.Id == request.MemberId);

            //var checkUserExist = await _context.Users
            //   .FirstOrDefaultAsync(q => q.UserName == request.UserName);

            if (checkMemberExist == null)
            {
                result.HasError = true;
                result.Messages.Add("Member does not exist!!!");
                return result;
            }
            //if (checkMemberExist.IsApproved)
            //{
            //    result.HasError = true;
            //    result.Messages.Add("Member is already approved!!!");
            //    return result;
            //}
            if (request.SubscriptionStarts == null)
            {
                result.HasError = true;
                result.Messages.Add("Subscription Starting Date can not be empty!!!");
                return result;
            }


            if (checkMemberExist != null)
            {
                try
                {

                    checkMemberExist.IsApproved = true; // request.IsApproved;
                    checkMemberExist.Note = request.Note;
                    checkMemberExist.MemberCategoryId = request.MemberCategoryId;
                    checkMemberExist.MembershipFee = request.MembershipFee;
                    checkMemberExist.SubscriptionFee = request.SubscriptionFee;
                    checkMemberExist.ApprovedBy = _currentUserService.Current().Id;
                    checkMemberExist.ApproveTime = DateTime.Now;

                    if (checkMemberExist.SubscriptionStarts == null && checkMemberExist.PaidTill == null && request.SubscriptionStarts != null)
                    {
                        checkMemberExist.SubscriptionStarts = request.SubscriptionStarts;
                        // Subtract one month to go to the previous month
                        DateTime previousMonth = request.SubscriptionStarts.GetValueOrDefault().AddMonths(-1);

                        // Get the last day of that month
                        int lastDay = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
                        //var paidTill= request.SubscriptionStarts.AddMonths(-1);
                        checkMemberExist.PaidTill = new DateTime(previousMonth.Year, previousMonth.Month, lastDay);


                    }


                    _context.MemberRegistrationInfos.Update(checkMemberExist);

                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        if (checkMemberExist.SubscriptionStarts != null && checkMemberExist.PaidTill != null && request.SubscriptionStarts != null)
                        {
                            var checkExist = _context.RoSubscriptionDueTemps
                                         .AsNoTracking()
                                         .Any(x => x.MemberId == checkMemberExist.Id);
                            if (!checkExist)
                            {

                                await _mediator.Send(new RSubscriptionDueByMemberCommand() { MemberId = checkMemberExist.Id });
                            }
                        }


                        result.HasError = false;
                        result.Messages.Add("Member Approve Status changed successfully!!");

                        return result;
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
                    result.Messages.Add("Something went wrong!!!");
                    return result;
                }
            }
            return result;
        }
    }
}
