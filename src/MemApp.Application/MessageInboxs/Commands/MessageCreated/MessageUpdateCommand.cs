using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.MessageInboxs.Commands.MessageUpdate;


public class MessageUpdateCommand : IRequest<Result>
{
    public int Id { get; set; }
}

public class MessageUpdateCommandHandler : IRequestHandler<MessageUpdateCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IUserLogService _userLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionHandler _permissionHandler;
    public MessageUpdateCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
    {
        _context = context;
        _mediator = mediator;
        _userLogService = userLogService;
        _currentUserService = currentUserService;
        _permissionHandler = permissionHandler;
    }
    public async Task<Result> Handle(MessageUpdateCommand request, CancellationToken cancellation)
    {
        try
        {
            //if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            //{
            //    throw new UnauthorizedAccessException();
            //}
            var result = new Result();

            var obj = await _context.MessageInboxs.FirstOrDefaultAsync(c => c.Id == request.Id);
           

            obj.IsRead = true;

            _context.MessageInboxs.Update(obj);

            await _context.SaveChangesAsync(cancellation);
            result.HasError = false;
            result.Succeeded = true;
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}