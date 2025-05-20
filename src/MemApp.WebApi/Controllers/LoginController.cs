using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Commands.ChangedPassword;
using MemApp.Application.Com.Commands.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Res.WebApi.Controllers;
using ResApp.Application.App.Commands;

namespace MemApp.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ApiNewControllerBase
    {
        public LoginController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> Authenticate(LoginCommand model)
        {
            model.IpAddress = GenerateIPAddress();
            var profile = await Mediator.Send(model);

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateDash(LoginCommand model)
        {
            model.IpAddress = GenerateIPAddress();
            var profile = await Mediator.Send(model);

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> LoginMember(ResMemberLoginCommand model)
        {
            model.IpAddress = GenerateIPAddress();
            var result = await Mediator.Send(model);
           
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> LoginMember(MemberLoginReq model)
        //{
        //    model.IpAddress = GenerateIPAddress();
        //    var result = await Mediator.Send(new MemberLoginCommand()
        //    {
        //        Model = model,
        //    });
        //    return Ok(result);
        //}

        [HttpPost]
        public async Task<TokenResponse> RefreshToken(RefreshTokenCommand model)
        {
            model.IpAddress = GenerateIPAddress();
            var profile = await Mediator.Send(model);
            return profile;
        }

        [HttpPost]
        public async Task<IActionResult> Revoke()
        {

            var result = await Mediator.Send(new RevokeCommand()
            {
                IpAddress = GenerateIPAddress()
            });
            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> GetOtp(ForgetPasswordReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(new ForgetPasswordCommand() { Model = model }));
        }

        [HttpPost]
        public async Task<IActionResult> NewPasswordByOtp(NewPasswordReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(new NewPasswordCommand() { Model = model }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveDeviceToken(SaveDeviceTokenCommand model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(model));
        }

        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
