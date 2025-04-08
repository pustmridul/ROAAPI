using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.MessageInboxs.Commands.MessageRead;


public class MessageReadCommand : IRequest<Result>
{
    public int Id { get; set; }
}

public class MessageReadCommandHandler : IRequestHandler<MessageReadCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IUserLogService _userLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionHandler _permissionHandler;
    public MessageReadCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
    {
        _context = context;
        _mediator = mediator;
        _userLogService = userLogService;
        _currentUserService = currentUserService;
        _permissionHandler = permissionHandler;
    }
    public async Task<Result> Handle(MessageReadCommand request, CancellationToken cancellation)
    {
        try
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            var obj = await _context.MessageInboxs
                .SingleOrDefaultAsync(q => q.Id == request.Id, cancellation);
            if (obj == null)
            {

                result.HasError = false;
                result.Messages.Add("Message not found");
                return result;
            }
            else
            {
                obj.IsRead = true;
                result.HasError = false;
                result.Messages.Add("AddOnsItem Updated");

                await _context.SaveChangesAsync(cancellation);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}