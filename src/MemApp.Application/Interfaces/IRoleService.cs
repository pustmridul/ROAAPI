using MemApp.Application.Extensions;
using MemApp.Application.Models.Requests;
using MemApp.Application.Models.Responses;

namespace MemApp.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Result<RoleDto>> CreateRole(RoleDto req);
        Task<ListResult<RoleDto>> GetAllRole();
    }
}