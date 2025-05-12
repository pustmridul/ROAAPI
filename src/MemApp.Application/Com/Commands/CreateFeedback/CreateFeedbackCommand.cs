using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;
using MemApp.Application.Extensions;
using MemApp.Domain.Enums;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Com.Commands.CreateFeedback
{
    public class CreateFeedbackCommand : IRequest<Result>
    {
        public FeedbackReq Model { get; set; } = new FeedbackReq();
    }

    public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IBroadcastHandler _broadcastHandler;

        public CreateFeedbackCommandHandler(IMemDbContext context, IBroadcastHandler broadcastHandler)
        {
            _context = context;
            _broadcastHandler = broadcastHandler;
        }
        public async Task<Result> Handle(CreateFeedbackCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.Feedbacks
                            .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    obj = new Feedback()
                    {
                        FeedbackDate = DateTime.Now,
                        Status = (int)FeedbackStatus.Unseen
                    };
                    _context.Feedbacks.Add(obj);
                    result.Succeeded = true;
                    result.Messages.Add("Created success");
                }

                obj.MemberId = request.Model.MemberId;
                obj.FeedbackCategoryId = request.Model.FeedbackCategoryId;
                obj.MemberShipNo = request.Model.MemberShipNo;
                obj.Message = request.Model.Message;


                await _context.SaveChangesAsync(cancellation);

                var FeedbackCategory = await _context.FeedbackCategories
                    .Include(c => c.RegisterMember)
                    .FirstOrDefaultAsync(c => c.Id == obj.FeedbackCategoryId);

                var mail = FeedbackCategory!.RegisterMember.Email;
                var mailBody = $"You have a feedback from {FeedbackCategory.RegisterMember.FullName}, Membershipno: {FeedbackCategory.RegisterMember.MembershipNo}. Feedback: {request.Model.Message}";
                if (!string.IsNullOrEmpty(mail))
                {
                  await  _broadcastHandler.SendEmail(mail, FeedbackCategory.Name, mailBody);
                }



            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.Messages.Add(ex.Message);
            }
            return result;
        }
    }
}
