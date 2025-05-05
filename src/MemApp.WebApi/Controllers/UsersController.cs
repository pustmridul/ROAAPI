using MediatR;
using MemApp.Application.Com.Commands.ChangedPassword;
using MemApp.Application.Com.Commands.CreateUser;
using MemApp.Application.Com.Commands.ResetPassword;
using MemApp.Application.Com.Commands.SaveUserPermission;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.GetAllUsers;
using MemApp.Application.Com.Queries.GetNavMenuByUserId;
using MemApp.Application.Com.Queries.GetRolesByUser;
using MemApp.Application.Com.Queries.GetUserMenus;
using MemApp.Application.Com.Queries.GetUserPermissions;
using MemApp.Application.Com.Queries.SaveRolesByUser;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.Com.Queries.GetCurrentUserDetails;
using ResApp.Application.Com.Queries.GetUserById;

namespace MemApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly IWebHostEnvironment _iWebHostEnvironment;
        private readonly IUserService _userService;
        private ISender _mediator = null!;

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        public UsersController(IUserService userService, IWebHostEnvironment iWebHostEnvironment)
        {
            _userService = userService;
            _iWebHostEnvironment = iWebHostEnvironment;
        }
      //  [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveUser(CreateUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserById(int userId = 0)
        {
            return Ok(await Mediator.Send(new GetUserByIdQuery()
            {
                UserId = userId
            }));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pageNo, int pageSize, string appId, string? searchText)
        {
            var result = await Mediator.Send(new GetAllUserQuery()
            {
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText
            });
            return Ok(result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetNavMenus(int userId = 0)
        {
            return Ok(await Mediator.Send(new GetNavMenusByUserIdQuery()
            {
                UserId = userId
            }));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<PermissionModelListVm> GetUserPermissionById(int userId)
        {
            return await Mediator.Send(new GetPermissionListQuery() { userId = userId });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost]
        public async Task<PermissionModelListVm> SaveUserPermissionById(UserPermissionReq models)
        {
            return await Mediator.Send(new SaveUserPermissionCommand() { model = models });
        }
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> SaveUserMenuMapById(List<UserMenuMapReq> models, int userId)
        //{
        //    return await Mediator.Send(new SaveUserMenuMapCommand() { models = models, UserId=userId });
        //}

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<Result> ResetPassword(int userId)
        {
            return await Mediator.Send(new ResetUserPasswordCommand() { UserId = userId });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangedPassword(ChangedPasswordCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserMenus(int userId = 0)
        {
            return Ok(await Mediator.Send(new GetUserMenuQuery()
            {
                UserId = userId
            }));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<UserLogListVm> GetUserLogs(int pageNo, int pageSize)
        {
            var result = await _userService.GetAllUserLogs(pageNo, pageSize);
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<UserLogListVm> GetUserLogsByUserId(int pageNo, int pageSize, string UserId)
        {
            var result = await _userService.GetAllUserLogsByUserId(pageNo, pageSize, UserId);
            return result;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<UsersRoleModelListVm> GetRoleByUserId(int UserId)
        {
            return await Mediator.Send(new GetRolesByUserQuery() { UserId = UserId });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPost]
        public async Task<UsersRoleModelListVm> SaveRoleByUserId(UsersRoleModelListVm model)
        {
            return await Mediator.Send(new SaveRolesByUserQuery() { Model = model });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserDetails()
        {
            return Ok( await Mediator.Send(new GetCurrentUserDetailsQuery()));
        }


    }
}
