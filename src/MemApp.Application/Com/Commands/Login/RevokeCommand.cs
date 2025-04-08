using MediatR;
using MemApp.Application.Com.Commands.UserConferences;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Commands.Login
{
    public class RevokeCommand : IRequest<Result>
    {
        public string IpAddress { get; set; } = string.Empty;
    }
    public class RevokeCommandHandler : IRequestHandler<RevokeCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        public RevokeCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IMediator mediator)
        {
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;

        }
        public async Task<Result> Handle(RevokeCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var username = _currentUserService.Username;
            var userList = await _context.RefreshTokens
                .Where(q => q.UserName == username && q.UserId== _currentUserService.UserId)
                .OrderByDescending(q => q.Id)
                .ToListAsync(cancellationToken);

            if (userList.Count == 0)
            {
                result.HasError=false;
                result.Messages.Add("Something went wrong!");
            }
            else
            {
                _context.RefreshTokens.RemoveRange(userList);

                await _mediator.Send(new CreateUserConferenceCommand()
                {
                    Model = new Models.UserConferenceReq()
                    {
                        UserId = _currentUserService.UserId,
                        UserName =_currentUserService.Username,
                        UserRefToken = userList.FirstOrDefault()?.RefToken,
                        UserToken = userList.FirstOrDefault()?.Token,
                        IpAddress = request.IpAddress,
                        AppId = userList.FirstOrDefault()?.AppId,
                        LogOutDate = DateTime.Now,
                        LogOutStatus = true
                    }
                });
                 result.HasError=false;
                result.Messages.Add("Revoke Succes");
            }

            return result;
           
        }
    }
}
