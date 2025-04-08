using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class CreateMemServiceCommand : IRequest<Result<MemServiceDto>>
    {
        public string? ImgFileUrl { get; set; }
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeTitle { get; set; }

        public string Title { get; set; }
        public string DisplayTitle { get; set; }

        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int ServiceTicketCount { get; set; }
    }

    public class MemServiceMemServiceCommandHandler : IRequestHandler<CreateMemServiceCommand, Result<MemServiceDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public MemServiceMemServiceCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService,  IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<MemServiceDto>> Handle(CreateMemServiceCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemServiceDto>();
            try
            {
                var obj = await _context.MemServices
                .SingleOrDefaultAsync(q => q.Id == request.Id);
                if (obj == null)
                {
                    if(!await _permissionHandler.HasRolePermissionAsync(1101))
                    {
                        result.HasError = true;
                        result.Messages.Add("MemService create Permission Denied");
                        return result;
                    }
                    obj = new MemService();
                    obj.IsActive = true;
                    _context.MemServices.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New MemService created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1102))
                    {
                        result.HasError = true;
                        result.Messages.Add("MemService Update Permission Denied");
                        return result;
                    }

                    result.HasError = false;
                    result.Messages.Add("MemService Updated");
                }

                obj.Title = request.Title;
                obj.ServiceTypeId = request.ServiceTypeId;

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                  //  result.Data.Title = obj.Title;
                  //  result.Data.Id = obj.Id; 
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("something wrong");
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Exception" + ex.Message);
            }


            return result;
        }
    }

}