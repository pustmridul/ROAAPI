using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.Donations.Commands;
using MemApp.Application.Donations.Models;
using MemApp.Application.Donations.Queries;

using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DonationsController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DonationsController(ISender mediator, IWebHostEnvironment webHostEnvironment) : base(mediator)
        {
         _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] DonationDto model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            
            var result = await Mediator.Send(new CreateDonationCommand { Model = model , WebRootPath= _webHostEnvironment.WebRootPath});
            return result.HasError ? BadRequest(result) : Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetDonationByIdQueries() { Id = id });
            return result.HasError ? BadRequest(result) : Ok(result);
        }
      // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParams pageParams)
        {
            var result = await Mediator.Send(new GetAllDonationQueries()
            {
                PageNo = pageParams.PageNo,
                PageSize = pageParams.PageSize,
                SearchText = pageParams.SearchText,
                
            });
            return result.HasError ? BadRequest(result) : Ok(result);
        }

    }
}
