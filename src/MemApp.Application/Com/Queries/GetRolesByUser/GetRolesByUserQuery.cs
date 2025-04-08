using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries.GetRolesByUser
{

    public class GetRolesByUserQuery : IRequest<UsersRoleModelListVm>
    {
        public int UserId { get; set; }
       
    }

    public class GetRolesByUserQueryHandler : IRequestHandler<GetRolesByUserQuery, UsersRoleModelListVm>
    {
        private readonly IMemDbContext _context;
        public GetRolesByUserQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<UsersRoleModelListVm> Handle(GetRolesByUserQuery request, CancellationToken cancellationToken)
        {
            var result = new UsersRoleModelListVm();
            var roleList =  await _context.Roles.Where(q => q.IsActive).ToListAsync(cancellationToken);
                
            var userRoleList =  await _context.UserRoleMaps.Where(q=>q.UserId==request.UserId
            && q.IsActive).ToListAsync(cancellationToken);

            foreach (var r in roleList)
            {
                var obj = new UsersRoleModel()
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                };
                var urol = userRoleList.FirstOrDefault(q => q.RoleId == r.Id);
                if (urol == null)
                {
                    obj.IsChecked = false;

                }
                else
                {
                    obj.IsChecked = true;
                }
                obj.UserId= request.UserId;
                

                result.DataList.Add(obj);
                
            }           

            return result;
        }
    }
}
