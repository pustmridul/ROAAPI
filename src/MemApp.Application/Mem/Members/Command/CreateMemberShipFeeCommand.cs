using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberShipFeeCommand : IRequest<Result<MemberShipFeeDto>>
    {
        
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string? DisplayName { get; set; }
        public int MemberTypeId { get; set; }
        public string? MemberTypeText { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsChecked { get; set; }
    }
    public class CreateMemberSheepFeeCommandHandler : IRequestHandler<CreateMemberShipFeeCommand, Result<MemberShipFeeDto>>
    {
        private readonly IMemDbContext _memdbcontext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;

        public CreateMemberSheepFeeCommandHandler(IMemDbContext memDbContext, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _memdbcontext = memDbContext;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<MemberShipFeeDto>> Handle(CreateMemberShipFeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }
                var result = new Result<MemberShipFeeDto>();
                var obj = await _memdbcontext.MemberShipFees
                    .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1601))
                    {
                        result.HasError = true;
                        result.Messages.Add("MembershipFees Create Permission Denied");
                        return result;
                    }
                    obj = new MemberShipFee();
                    obj.IsActive = true;
                    obj.IsCurrent = true;
                    obj.DisplayName = request.DisplayName;
                    obj.Amount = request.Amount;
                    obj.Title = request.Title;
                    obj.MemberTypeId = request.MemberTypeId;

                    _memdbcontext.MemberShipFees.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New MembershipFees created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(1602))
                    {
                        result.HasError = true;
                        result.Messages.Add("MembershipFees Update Permission Denied");
                        return result;
                    }

                    result.HasError = false;
                    result.Messages.Add("MembershipFees Updated");
                }
                obj.Title = request.Title;
                obj.DisplayName = request.DisplayName;

                await _memdbcontext.SaveChangesAsync(cancellationToken);
                
               return result;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }
    }
}
