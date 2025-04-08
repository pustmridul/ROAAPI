using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.com;
using MemApp.Application.Extensions;
using Microsoft.AspNetCore.Http;
using MemApp.Application.Interfaces;
using FirebaseAdmin.Messaging;
using Hangfire;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Domain.Enums;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;

namespace MemApp.Application.Com.Commands.CreateReply
{
    public class CreateReplyCommand : IRequest<Result>
    {
        public ReplyReq Model { get; set; } = new ReplyReq();
    }

    public class CreateReplyCommandHandler : IRequestHandler<CreateReplyCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;
        public CreateReplyCommandHandler(
            ICurrentUserService currentUserService,
            IMemDbContext context,
            IMediator mediator,
            IBackgroundJobClientV2 backgroundJobClientV2
            )
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<Result> Handle(CreateReplyCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {

                var obj = await _context.Replys
                            .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                var feedback = await _context.Feedbacks.SingleOrDefaultAsync(c => c.Id == request.Model.FeedbackId);

                if (obj == null)
                {
                    obj = new Reply()
                    {
                        FeedbackId= request.Model.FeedbackId,
                        UserId= _currentUserService.UserId,
                        UserName= _currentUserService.Username,
                        ReplyDate = DateTime.Now,
                        Message = request.Model.Message,
                    };
                  
                    _context.Replys.Add(obj);
                    result.Succeeded = true;
                    result.Messages.Add("Created success");
                }

               
                obj.Message = request.Model.Message;
                
                await _context.SaveChangesAsync(cancellation);


                var messageObj = new MessageInboxCreateDto
                {
                    MemberId = feedback.MemberId,
                    Message = $"{_currentUserService.Username} replied to your feedback, {request.Model.Message}",
                    TypeId = MessageInboxTypeEnum.FeedBackReply,
                    IsRead = false,
                    IsAllMember = false,

                };
                _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));

            }
            catch (Exception ex) { 
                result.Succeeded = false;
                result.Messages.Add(ex.Message);
            }
            return result;
        }
        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
        }
    }
}
