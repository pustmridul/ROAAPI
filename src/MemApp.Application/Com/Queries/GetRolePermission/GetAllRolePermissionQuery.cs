using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.Requests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.GetRolePermission
{


    public class GetAllRolePermissionQuery : IRequest<Dictionary<string, List<PermissionDetailVm>>>
    {
       
    }

    public class GetAllRolePermissionQueryHandler : IRequestHandler<GetAllRolePermissionQuery, Dictionary<string, List<PermissionDetailVm>>>
    {
        private readonly IMemDbContext _context;

        public GetAllRolePermissionQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, List<PermissionDetailVm>>> Handle(GetAllRolePermissionQuery request, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, List<PermissionDetailVm>>();

            try

            {
                //var grouped = await _context.Permissions
                //    .Where(p => p.IsActive)
                //    .GroupBy(p => p.ModuleName)
                //    .Select(g => new
                //    {
                //        Module = g.Key,
                //        Permissions = g.Select(p => new PermissionDetailVm
                //        {
                //            Operation = p.OperationName,
                //            PermissionNo = 0,// p.PermissionNo.ToString()
                //        }).ToList()
                //    })
                //    .ToListAsync(cancellationToken);

                var data = await _context.Permissions
                            .Where(p => p.IsActive)
                            .GroupBy(p => new { p.ModuleName, p.OperationName })
                            .Select(g => new
                            {
                                Module = g.Key.ModuleName,
                                Action = g.Key.OperationName,
                                Permission = g.Select(x => x.PermissionNo).FirstOrDefault()
                            })
                            .ToListAsync();

                result = data
                   .GroupBy(x => x.Module)
                   .ToDictionary(
                       g => g.Key,
                       g => g.Select(x => new PermissionDetailVm
                       {
                           Name = x.Action,
                           PermissionNo = x.Permission
                       }).ToList()
                   );

                //var list = await _context.Permissions.Where(x=>x.IsActive).Select( g=> new PermissionDetailVm
                //{
                //    Name=g.ModuleName,
                //    Operation=g.OperationName,
                //    PermissionNo=g.PermissionNo,
                //}).ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
