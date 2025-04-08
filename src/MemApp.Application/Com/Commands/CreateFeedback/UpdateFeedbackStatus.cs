using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Commands.CreateFeedback
{
    public class UpdateFeedbackStatus : IRequest<Result>
    {
        public FeedbackStatusReq Model { get; set; } = new FeedbackStatusReq();
    }

    public class UpdateFeedbackStatusHandler : IRequestHandler<UpdateFeedbackStatus, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;


        public UpdateFeedbackStatusHandler(IFileSaveService fileService, IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;

        }
        public async Task<Result> Handle(UpdateFeedbackStatus request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();

            var obj = await _context.Feedbacks
                .SingleOrDefaultAsync(q => q.Id == request.Model.FeedbackId, cancellation);
            if (obj != null)
            {
                obj.Status = request.Model.FeedbackStatus;
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    result.Succeeded = true;
                    result.Messages.Add("Feedback status updated succesfully");
                }
                else
                {
                    result.Succeeded = false;
                    result.Messages.Add("something wrong");
                }
                return result;
            }

            result.Succeeded = false;
            result.Messages.Add("something wrong");

            return result;



        }
    }

}
