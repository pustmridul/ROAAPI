using MediatR;
using MemApp.Application.Mem.MemberProfessions.Command;
using MemApp.Application.Mem.MemberProfessions.Queries;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberProfessionsController : ApiControllerBase
    {
        public MemberProfessionsController(ISender sender): base(sender) { }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberProfessionCommand command)
        {
            if (command == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(command);
            return Ok(result); 
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetMemberProfessionByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllMemberProfessionQuery());
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberProfessionCommand() { Id = id });
            return Ok(result);
        }


    }
}
