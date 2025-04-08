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
    public class CommunicationController : ApiControllerBase
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommunicationController(IWebHostEnvironment webHostEnvironment, ISender sender) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveNotice([FromForm] CreateNoticeCommand command)
        {
            if (command == null)
            {
                return BadRequest();
            }
            command.webHostEnvironment = _webHostEnvironment.WebRootPath;
            var result = await Mediator.Send(command);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetNoticeById(int id)
        {
            var result = await Mediator.Send(new GetNoticeByIdQuery() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllNotice(int pageNo = 1, int pageSize = 1000)
        {
            var result = await Mediator.Send(new GetAllNoticeQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveNotice(int id)
        {
            var result = await Mediator.Send(new DeleteNoticeCommand() { Id = id });
            return Ok(result);
        }
    }
}
