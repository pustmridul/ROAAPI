using MediatR;
using MemApp.Application.Donates.Commands;
using MemApp.Application.Donates.Models;
using MemApp.Application.Donates.Queries;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DonatesController : ApiControllerBase
    {
        public DonatesController(ISender mediator) : base(mediator) { }
   
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(DonateDto model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateDonateCommand { Model = model });
            return result.HasError ?  BadRequest(result) : Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBymemberId(int memberId)
        {
            var result = await Mediator.Send(new GetDonateByMemberIdQueries() { MemberId = memberId });
            return result.HasError ? BadRequest(result) : Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result= await Mediator.Send(new GetDonateByIdQueries() { Id=id});
            return result.HasError ? BadRequest(result) : Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParams pageParams)
        {
            var  result = await Mediator.Send(new GetAllDonateQueries()
            {
                PageNo = pageParams.PageNo,
                PageSize = pageParams.PageSize,
                SearchText = pageParams.SearchText,
            });
            return result.HasError ? BadRequest(result) : Ok(result);
        }
       
       
    }
}
