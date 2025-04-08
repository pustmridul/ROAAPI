using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberAddresss.Command;
using MemApp.Application.Mem.MemberAddresss.Queries;
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
    public class MemberAddresssController : ApiControllerBase
    {
        public MemberAddresssController(ISender sender) : base(sender) { }
    
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberAddressCommand model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(model);
            return Ok(result); 
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetMemberAddressByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(string addressType)
        {
            var result = await Mediator.Send(new GetAllMemberAddressQuery());
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberAddressCommand() { Id = id });
            return Ok(result);
        }
    }
}
