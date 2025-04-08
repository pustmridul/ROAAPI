using MediatR;
using MemApp.Application.Mem.MemServices.Command;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.MemServices.Queries;
using MemApp.Application.Models;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemServicesController : ApiControllerBase
    {
        public MemServicesController(ISender sender): base(sender) { }
      
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(CreateMemServiceCommand model)
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
            var result = await Mediator.Send(new GetMemServiceByIdQuery() { Id=id});
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetAllMemServiceQuery()
            {
                PageNumber= pageNo,
                PageSize= pageSize
            });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllVenueWithAvailable(DateTime? selectedDate)
        {
            if(selectedDate == null)
            {
                return BadRequest();
            }
            else
            {
                var result = await Mediator.Send(new GetAllVenueWithAvailableQuery()
                {
                    SelectedDate = selectedDate
                });
                return Ok(result);
            }
           
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemServiceCommand() { Id = id });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServiceOnly(int pageNo, int pageSize)
        {
            var result = await Mediator.Send(new GetAllMemServiceOnlyQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
    }
}
