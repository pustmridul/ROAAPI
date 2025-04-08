using MediatR;
using MemApp.Application.Mem.Communication.Command;
using MemApp.Application.Mem.Communication.Models;
using MemApp.Application.Mem.Communication.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LiquorPermitController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LiquorPermitController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveLiquorPermit([FromForm] LiquorPermitModelReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateLiquorPermitCommand { Model = model, webHostEnvironment = _webHostEnvironment.WebRootPath });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetLiquorPermitByMemberId(int memberId)
        {
            var result = await Mediator.Send(new GetLiquorPermitByIdQuery() { MemberId = memberId });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllLiquorPermit(int pageNo = 1, int pageSize = 1000)
        {
            var result = await Mediator.Send(new GetAllLiquorPermitQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveLiquorPermit(int id)
        {
            var result = await Mediator.Send(new DeleteLiquorPermitCommand() { Id = id });
            return Ok(result);
        }
    }
}
