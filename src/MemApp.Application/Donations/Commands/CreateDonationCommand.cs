using MediatR;
using MemApp.Application.Donations.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donations.Commands;


public class CreateDonationCommand : IRequest<Result>
{
    public DonationDto Model { get; set; } = new DonationDto();
    public string WebRootPath { get; set; } = string.Empty;
}

public class CreateDonationCommandHandler : IRequestHandler<CreateDonationCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IUserLogService _userLogService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionHandler _permissionHandler;
    private readonly IFileService _fileService;
    public CreateDonationCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService,
        IPermissionHandler permissionHandler, IFileService fileService)
    {
        _context = context;
        _mediator = mediator;
        _userLogService = userLogService;
        _currentUserService = currentUserService;
        _permissionHandler = permissionHandler;
        _fileService = fileService;
    }
    public async Task<Result> Handle(CreateDonationCommand request, CancellationToken cancellation)
    {
        try
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();

            var obj = await _context.Donations.SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);
            if (obj == null)
            {
                obj = new Donation()
                {
                    IsActive = true,
                };
                
                _context.Donations.Add(obj);
              

            }
            obj.Title = request.Model.Title;
            obj.Description = request.Model.Description;
            obj.StartDate = request.Model.StartDate;
            obj.EndDate = request.Model.EndDate;
            obj.FileUrl = request.Model.FileUrl;
            
            obj.IsFixed = request.Model.IsFixed;
            obj.Amount = request.Model.Amount;
            
            await _context.SaveChangesAsync(cancellation);

            if (request.Model.DonationFile != null)
            {
                var fileObj = new DonationUploadModel
                {
                    DonationId = obj.Id,
                    SubDirectory = "Donation",
                    file = request.Model.DonationFile
                };
                await _fileService.UploadDonationFile(fileObj, request.WebRootPath);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}
