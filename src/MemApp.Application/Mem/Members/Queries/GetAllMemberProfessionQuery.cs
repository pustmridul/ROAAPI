using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberProfessions.Queries
{
    public class GetAllMemberProfessionQuery : IRequest<ListResult<MemberProfessionDto>>
    {
        public int Id { get; set; }
    }
    public class GetAllMemberProfessionQueryHandler : IRequestHandler<GetAllMemberProfessionQuery, ListResult<MemberProfessionDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberProfessionQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemberProfessionDto>> Handle(GetAllMemberProfessionQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberProfessionDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(1503))
            {
                result.HasError = true;
                result.Messages?.Add("Permission Denied When Access to Member Profession VIEW");
                return result;
            }

            var data = await _context.MemberProfessions.Where(q=>q.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count= data.Count;
                result.Data =data.Select(s=> new MemberProfessionDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
            }

            return result;
        }
    }
}
