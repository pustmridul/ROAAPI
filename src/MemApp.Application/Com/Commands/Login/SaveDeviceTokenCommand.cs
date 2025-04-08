using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Commands.Login;

public class SaveDeviceTokenCommand : IRequest<Result>
{
    public string DeviceToken { get; set; }
    public int UserId { get; set; }
}
public class SaveDeviceTokenCommandHandler : IRequestHandler<SaveDeviceTokenCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    public SaveDeviceTokenCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;

    }
    public async Task<Result> Handle(SaveDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var result = new Result();
        try
        {
            if (!(request.DeviceToken == null || request.DeviceToken == "" || request.DeviceToken == "null"))
            {
                var notificationTokenObj = await _context.NotificationTokens
              .SingleOrDefaultAsync(q => q.UserId == request.UserId && q.DeviceToken == request.DeviceToken, cancellationToken);

                if (notificationTokenObj == null)
                {
                    notificationTokenObj = new NotificationToken()
                    {
                        UserId = request.UserId,
                    };
                    _context.NotificationTokens.Add(notificationTokenObj);
                }

                notificationTokenObj.DeviceToken = request.DeviceToken;
                await _context.SaveChangesAsync(cancellationToken);
                result.HasError = false;
                result.Messages.Add("Device Token Save Success");
            }
        }
        catch (Exception ex)
        {
            result.HasError = true;
            result.Messages.Add(ex.Message);
        }

        return result;

    }
}
