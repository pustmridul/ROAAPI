using Google.Apis.Logging;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MemApp.Application.Exceptions;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TRequest> _logger;

    public PerformanceBehaviour(ICurrentUserService currentUserService, ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _currentUserService = currentUserService;
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 5000)
            {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var userName = _currentUserService.Username;

            

            _logger.LogWarning("FlexPosly Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
