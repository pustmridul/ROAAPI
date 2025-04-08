using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using System.ComponentModel;

namespace MemApp.Application.Com.Queries.GetUserPermissions
{
    public class GetPermissionListQuery : IRequest<PermissionModelListVm>
    {
        public int userId { get; set; }
    }
    public class GetPermissionListQueryHandler : IRequestHandler<GetPermissionListQuery, PermissionModelListVm>
    {
        private readonly IMemDbContext _context;

        public GetPermissionListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async  Task<PermissionModelListVm> Handle(GetPermissionListQuery request , CancellationToken cancellationToken)
        {
            var userPermissionList = _context.UserPermissions.Where(q => q.UserId == request.userId).ToList();

            var permissionList = new PermissionModelListVm();
            var parents = typeof(Collection.PermissionCollection)
                 .GetNestedTypes()
                 .Where(t => t.IsSealed)
                 .ToArray();

            foreach (var parent in parents)
            {
                permissionList.PermissionList.Add(BuildPermission(parent, null));
            }

            foreach (var p in permissionList.PermissionList)
            {
                foreach (var p1 in p.Childs)
                {
                    p.IsChecked = true;
                    p.UserId = request.userId;
                    var up = userPermissionList.FirstOrDefault(f => f.PermissionNo == p1.ID);
                    if (up == null)
                    {
                        foreach(var p2 in p1.Childs)
                        {
                            var up1 = userPermissionList.FirstOrDefault(f => f.PermissionNo == p2.ID);
                            if(up1 == null)
                            {
                                p1.IsChecked = false;
                            }
                            else
                            {
                                p2.IsChecked = true;
                                p2.UserId = request.userId;
                            }
                        }
                        p.IsChecked = false;
                    }
                    else
                    {
                        p1.IsChecked = true;
                        p1.UserId = request.userId;
                    }
                }
            }
            return permissionList;
        }

        private PermissionModel BuildPermission(Type type, int? parentId)
        {
            var parent = new PermissionModel();

            int id = Convert.ToInt32((type.GetCustomAttributes(typeof(DescriptionAttribute), false)[0] as DescriptionAttribute).Description);

            parent.ID = id;
            parent.Name = type.Name;
            parent.ParentID = parentId;

            parent.IsChecked = false;
            parent.IsExpanded = false;

            parent.Childs = new List<PermissionModel>();

            var fields = type.GetFields();
            foreach (var field in fields)
            {
                var child = new PermissionModel
                {
                    ID = Convert.ToInt32(field.GetValue(field)),
                    Name = field.Name,
                    ParentID = parent.ID
                };
                parent.Childs.Add(child);
            }

            var nestedChilds = type.GetNestedTypes().ToArray();
            foreach (var child in nestedChilds)
            {
                parent.Childs.Add(BuildPermission(child, parent.ID));
            }
            return parent;
        }

    }
}
