using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Colleges.Command
{
    public class CreateCollegeCommand : IRequest<Result<CollegeDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public int Id { get; set; }
        public string? OldId { get; set; }

    }

    public class CreateCollegeCommandHandler : IRequestHandler<CreateCollegeCommand, Result<CollegeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateCollegeCommandHandler(IMemDbContext context, IMediator mediator,  ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<CollegeDto>> Handle(CreateCollegeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<CollegeDto>();
            var obj = await _context.Colleges
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {
                if(!await _permissionHandler.HasRolePermissionAsync(1201))
                {
                    result.HasError = true;
                    result.Messages.Add("New College Creation Permission Denied");
                    return result;
                }
                obj = new College();
                obj.IsActive = true;
                _context.Colleges.Add(obj);
                result.HasError = false;
                result.Messages.Add("New College Created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(1202))
                {
                    result.HasError = true;
                    result.Messages.Add("College Update Permission Denied");
                    return result;
                }
                result.HasError = false;
                result.Messages.Add("College Updated");
            }

            obj.Name = request.Name;
            obj.Code = request.Code;
            obj.OldId = request.OldId;
            await _context.SaveChangesAsync(cancellation);
         
           
            return result;
        }
    }

}
