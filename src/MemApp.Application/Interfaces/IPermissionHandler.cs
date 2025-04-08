using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IPermissionHandler
    {
        bool HasPermission(int permissionId);
        bool HasRolePermission(int permissionId);

        Task<bool> HasPermissionAsync(int permissionId);

        Task<bool> HasRolePermissionAsync(int permissionNo);
        Task<bool> IsTempMember();
    }
}
