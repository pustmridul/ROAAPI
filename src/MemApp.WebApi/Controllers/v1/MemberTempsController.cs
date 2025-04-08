using MediatR;
using MemApp.Application.Mem.MemberTemps.Command;
using MemApp.Application.Mem.MemberTemps.Models;
using MemApp.Application.Mem.MemberTemps.Queries;
using MemApp.Application.Models;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberTempController : ApiControllerBase
    {
        private IWebHostEnvironment _webHostEnvironment;
        public MemberTempController(IWebHostEnvironment webHostEnvironment, ISender sender): base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] MemberTempReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            model.IpAddress =  GenerateIPAddress();
            var result = await Mediator.Send(new CreateMemberTempCommand { Model = model, WebRootPath=_webHostEnvironment.WebRootPath });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetMemberTempByIdQuery() { Id=id});
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNo=1, int pageSize=1000)
        {
            var result = await Mediator.Send(new GetAllMemberTempQuery()
            {
                PageNumber= pageNo,
                PageSize= pageSize
            });
            return Ok(result);
        }
       
        [HttpDelete]
        public async Task<IActionResult> Remove(int id) 
        {
            var result = await Mediator.Send(new DeleteMemberTempCommand() { Id = id });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> AcceptTempMember(int MemberId)
        {
            var result = await Mediator.Send(new UpdateMemberTempCommand { MemberId = MemberId });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> RejectTempMember(int MemberId)
        {
            var result = await Mediator.Send(new RejectMemberTempCommand { MemberId = MemberId });
            return Ok(result);
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
