using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberAddressCommand : IRequest<Result<MemberAddressDto>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class MemberAddressMemberAddressCommandHandler : IRequestHandler<CreateMemberAddressCommand, Result<MemberAddressDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public MemberAddressMemberAddressCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<MemberAddressDto>> Handle(CreateMemberAddressCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemberAddressDto>();
            var obj = await _context.MemberAddresses
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {
                //if(!await _permissionHandler.HasRolePermissionAsync())
                //{
                //    result.HasError = true;
                //    result.Messages.Add("Create Member Address Permission Denied");
                //    return result;
                //}
                obj = new MemberAddress();
                obj.IsActive = true;
                _context.MemberAddresses.Add(obj);
                result.HasError = false;
                result.Messages.Add("New MemberAddress created");
            }
            else
            {
                result.HasError = false;
                result.Messages.Add("MemberAddress Updated");
            }

            obj.Title = request.Title;
            obj.Code = request.Code;
            obj.Description = request.Description;
            obj.Type = request.Type;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

               // result.Data.Title = obj.Title;
              //  result.Data.Id = obj.Id;
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
            }

            return result;
        }
    }

}