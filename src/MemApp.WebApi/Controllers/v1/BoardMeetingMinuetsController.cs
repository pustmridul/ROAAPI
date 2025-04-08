using MediatR;
using MemApp.Application.Mem.BoardMeetingMinuets.Command;
using MemApp.Application.Mem.BoardMeetingMinuets.Models;
using MemApp.Application.Mem.BoardMeetingMinuets.Queries;
using MemApp.Application.Models;
using MemApp.Domain.Entities.com;
using MemApp.Reporting;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MemApp.Controllers.v1.com
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoardMeetingMinuetsController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BoardMeetingMinuetsController(ISender mediator, IWebHostEnvironment webHostEnvironment) : base(mediator)
        {
            _webHostEnvironment = webHostEnvironment;
        }
       
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result= await Mediator.Send(new GetBoardMeetingMinuetByIdQuery() { Id=id, WebRootPath= _webHostEnvironment.WebRootPath});
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNo=1, int pageSize=1000)
        {
            var result= await Mediator.Send(new GetAllBoardMeetingMinuetQuery()
            {
                PageNumber= pageNo,
                PageSize= pageSize,
                WebRootPath= _webHostEnvironment.WebRootPath
            });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result= await Mediator.Send(new DeleteBoardMeetingMinuetCommand() { Id = id });
            return Ok(result);
        }
    }
}
