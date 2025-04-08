using MemApp.Application.Mem.MemberTypes.Command;
using MemApp.Application.Mem.MemberTypes.Queries;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberTypesController : ApiControllerBase
    {
        public MemberTypesController(ISender sender) : base(sender) { }
    
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemberTypeCommand command)
        {
            if (command == null)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
            //var result = await Mediator.Send(new CreateMemberTypeCommand { Model = model });
            //return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result = await Mediator.Send(new GetMemberTypeByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await Mediator.Send(new GetAllMemberTypeQuery());
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberTypeByCategoryId(int categoryPatternID)
        {
            var result = await Mediator.Send(new GetMemberTypeByICategorydQuery() { CategoryId = categoryPatternID });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id) 
        {
            var result = await Mediator.Send(new DeleteMemberTypeCommand() { Id = id });
            return Ok(result);
        }


    }
}
