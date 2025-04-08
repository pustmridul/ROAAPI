using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Queries
{
    public class GetAllMemberShipFeeQuery : IRequest<ListResult<MemberShipFeeDto>>
    {
        public int MemberTypeId { get; set; }
    }

    public class GetAllMemberShipFeeQueryHandler : IRequestHandler<GetAllMemberShipFeeQuery, ListResult<MemberShipFeeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberShipFeeQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemberShipFeeDto>> Handle(GetAllMemberShipFeeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberShipFeeDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(1603))
            {
                result.HasError = true;
                result.Messages?.Add("MemberShip Fee Viewing Permission Denied");
                return result;
            }
            var data = await _context.MemberShipFees
                .Include(i=>i.MemberType)
                .Where(q=>q.IsActive && (request.MemberTypeId>0 ? q.MemberTypeId== request.MemberTypeId : true))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = false;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.Count;
                result.Data = data.Select(s => new MemberShipFeeDto
                {
                    
                    Id = s.Id,
                    Title = s.Title, 
                    Amount = s.Amount,
                    DisplayName= s.DisplayName,
                    LastUpdated= s.LastModifiedOn,
                    MemberTypeText= s.MemberType.Name,
                    MemberTypeId = s.MemberTypeId,
                    IsChecked =s.IsCurrent
                }).ToList();
            }

            return result;
        }
    }
}
