using MediatR;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemberStatuss.Command;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberStatussController : ApiControllerBase
    {
        public MemberStatussController(ISender sender) : base(sender)
        {

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberStatusCommand command)
        {
            //if (model == null)
            //{
            //    return BadRequest();
            //}
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetMemberStatusByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllMemberStatusQuery());
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberStatusCommand() { Id = id });
            return Ok(result);
        }


    }
}
