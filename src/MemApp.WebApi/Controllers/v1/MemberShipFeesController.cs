using MemApp.Application.Mem.Members.Queries;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using Microsoft.AspNetCore.Mvc;
using MemApp.Application.Models;
using Microsoft.AspNetCore.Authorization;
using MemApp.Application.Extensions;
using MediatR;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberShipFeesController : ApiControllerBase
    {
        public MemberShipFeesController(ISender sender): base(sender) { }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberShipFeeCommand command)
        {
            if (command == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet]
        public async Task<Result<MemberShipFeeDto>> GetMemberShipFeesById(int id)
        {
            return await Mediator.Send(new GetMemberShipFeeByIdQuery() { Id = id });
        }
        [HttpGet]
        public async Task<ListResult<MemberShipFeeDto>> GetAll(int memberTypeId)
        {
            return await Mediator.Send(new GetAllMemberShipFeeQuery() { MemberTypeId= memberTypeId});
        }
        [Authorize]
        [HttpDelete]
        public async Task<Result> Remove(int id) 
        {
            return await Mediator.Send(new DeleteMemberShipFeeCommand() { Id = id });
        }

    }
}
