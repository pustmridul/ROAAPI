using MediatR;
using MemApp.Application.Com.Queries.GetNavMenuByUserId;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.Requests;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Com.Commands.SaveRolePermission
{
    public class SaveRolePermissionCommand : IRequest<RolePermissionRes>
    {
        public RolePermissionRes Model { get; set; } = new RolePermissionRes();
    }

    public class SaveRolePermissionCommandHandler : IRequestHandler<SaveRolePermissionCommand, RolePermissionRes>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;

        public SaveRolePermissionCommandHandler(
            IMemDbContext context,
            IMediator mediator
            )
        {
            _context = context;
            _mediator = mediator;

        }


        //public async Task<RolePermissionRes> Handle(SaveRolePermissionCommand request, CancellationToken cancellation)
        //{

        //    var rolePermission = new List<RolePermissionMap>();
        //    var existingPermissionNo = new List<PermissionVm>();

        //    foreach (var permission in request.Model.PermissionList)
        //    {
        //        //existingPermissionNo = _context.RolePermissionMaps.Where(c => !request.Model.PermissionList.Select(c => c.PermissionNo).Contains(c.PermissionNo));
        //    }



        //        var addableRolePermissions = _context.RolePermissionMaps.Where(c => !request.Model.PermissionList.Select(c => c.PermissionNo).Contains(c.PermissionNo));

        //    if(addableRolePermissions!=null && addableRolePermissions.Count() > 0)
        //    {
        //        foreach (var permission in addableRolePermissions)
        //        {
        //            var obj = new RolePermissionMap()
        //            {
        //                RoleId = request.Model.RoleId,
        //                PermissionNo = permission.PermissionNo,
        //                IsActive = permission.IsActive
        //            };

        //            rolePermission.Add(obj);


        //        }
        //    }









        //    await _context.SaveChangesAsync(cancellation);


        //    return await _mediator.Send(new GetRolePermissionQuery()
        //    {
        //        RoleId = request.Model.RoleId
        //    });
        //}




        public async Task<RolePermissionRes> Handle(SaveRolePermissionCommand request, CancellationToken cancellation)
        {

            var rolePermission = new List<RolePermissionMap>();

            foreach (var d in request.Model.PermissionList)
            {
                if (d.IsChecked)
                {
                    var obj = await _context.RolePermissionMaps
                        .SingleOrDefaultAsync(q => q.PermissionNo == d.PermissionNo
                        && q.RoleId == request.Model.RoleId, cancellation);
                  
                    if(obj == null)
                    {
                        obj = new RolePermissionMap()
                        {
                            RoleId = request.Model.RoleId,
                            PermissionNo = d.PermissionNo,
                            IsActive = true
                        };
                        rolePermission.Add(obj);
                    }         
                    else
                    {
                        obj.IsActive = d.IsChecked;                     
                    }
                    foreach(var d1 in d.PermissionDetailVms)
                    {
                        var objChild = await _context.RolePermissionMaps.SingleOrDefaultAsync(q=>q.RoleId== request.Model.RoleId 
                        && q.PermissionNo== d1.PermissionNo, cancellation);
                        if(objChild == null)
                        {
                            objChild = new RolePermissionMap()
                            {
                                RoleId = request.Model.RoleId,
                                PermissionNo = d1.PermissionNo,
                                IsActive = true
                            };
                            rolePermission.Add(objChild);
                        }
                        else
                        {
                            objChild.IsActive = d1.IsChecked;
                        }
                    }
                }
                else
                {
                    var obj2 = await _context.RolePermissionMaps
                        .SingleOrDefaultAsync(q => q.PermissionNo == d.PermissionNo && q.RoleId == request.Model.RoleId, cancellation);
                    if (obj2 != null)
                    {
                        obj2.IsActive = false;

                        foreach (var d1 in d.PermissionDetailVms)
                        {

                            var objChild2 = await _context.RolePermissionMaps.SingleOrDefaultAsync(q => q.PermissionNo == d1.PermissionNo
                            && q.RoleId == request.Model.RoleId, cancellation);

                            if (objChild2!=null)
                            {
                                objChild2.IsActive = false;
                            }
                        }
                    }
                    
                }

            }

            if (rolePermission.Count > 0)
            {
                _context.RolePermissionMaps.AddRange(rolePermission);

            }


            await _context.SaveChangesAsync(cancellation);


            return await _mediator.Send(new GetRolePermissionQuery()
            {
                RoleId = request.Model.RoleId
            });
        }
    }

}
