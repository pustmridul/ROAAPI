using MediatR;
using MemApp.Application.Com.Commands.SaveRolePermission;
using MemApp.Application.Com.Queries.GetNavMenuByUserId;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.Requests;
using MemApp.Application.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Res.Domain.Core;

namespace MemApp.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RolesController : ApiControllerBase
    {
        private readonly IRoleService _RoleService;
        public RolesController(IRoleService RoleService, ISender sender) : base(sender)
        {
            _RoleService = RoleService;

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Result<RoleDto>>> CreateRole(RoleDto Role)
        {
           
            var result = await _RoleService.CreateRole(Role);
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllRole()
        {
            var result = await _RoleService.GetAllRole();
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<RolePermissionRes> GetRoleById(int roleId)
        {
            var data = await Mediator.Send(new GetRolePermissionQuery() { RoleId = roleId });
            return data;
        }
        [Authorize]
        [HttpPost]
        public async Task<RolePermissionRes> SaveRolePermissionById(RolePermissionRes model)
        {
            return await Mediator.Send(new SaveRolePermissionCommand() { Model = model });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetRolePermission()
        {
            var groupedPermissions = RolePermission.ApiPermissions
                 .GroupBy(
                     p => p.Key.Split('.').First(), // Use string as the key
                     p => new
                     {
                         Action = p.Key.Split('.').Last(), // The second part of the key (e.g., "Get")
                         Permissions = p.Value
                     })
                 .ToDictionary(
                     g => g.Key, // This is now a string
                     g => g.ToList()
                 );

            return Ok(groupedPermissions);
        }

    }
}
