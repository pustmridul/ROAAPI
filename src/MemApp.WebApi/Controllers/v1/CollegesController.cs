using Azure.Core;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Colleges.Command;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Colleges.Queries;
using MemApp.Application.Mem.Notification;
using MemApp.Application.Models.DTOs;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CollegesController : ApiControllerBase
    {

        private INotificationService _notificationService;
        public CollegesController(INotificationService notificationService, ISender sender) : base(sender)
        {
            _notificationService= notificationService;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save( CreateCollegeCommand command)
        {        
            return Ok(await Mediator.Send(command));
        }
        [HttpGet]
        public async Task<IActionResult> GetAlleById(int id)
        {
            var result= await Mediator.Send(new GetCollegeByIdQuery() { Id=id});
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CollegeSearchModel query)
        {
            var result= await Mediator.Send(new GetAllCollegeQuery()
            {
                PageNumber= query.PageNo,
                PageSize= query.PageSize
            });
            return Ok(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result= await Mediator.Send(new DeleteCollegeCommand() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<FileResult> Export()
        {
            var vm = await Mediator.Send(new ExportCollegesQuery());

            return File(vm.Content, vm.ContentType, vm.FileName);
        }
    }
}
