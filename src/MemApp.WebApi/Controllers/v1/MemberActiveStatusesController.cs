using MemApp.Application.Mem.MemberStatuss.Command;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using MemApp.Application.Mem.Colleges.Command;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberActiveStatussController : ApiControllerBase
    {
        public MemberActiveStatussController(ISender sender) : base(sender) { }
   
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberActiveStatusCommand command)
        {
            if (command == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetActiveStatusMemberById(int id)
        {
            var result = await Mediator.Send(new GetMemberActiveStatusByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllMemberActiveStatusQuery());
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberActiveStatusCommand() { Id = id });
            return Ok(result);
        }
    }
}
