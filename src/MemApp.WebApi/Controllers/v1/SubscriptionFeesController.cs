using MediatR;
using MemApp.Application.Mem.Subscription.Command;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Application.Mem.Subscription.Queries;
using MemApp.Application.Mem.Subscriptions.Command;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscriptionFeesController : ApiControllerBase
    {
        public SubscriptionFeesController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> Save(SubscriptionFeeReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateSubscriptionFeeCommand { Model = model });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDataPaginate(int pageNumber = 1, int pageSize = 10, string SearchText = "")
        {
            var result = await Mediator.Send(new GetAllSubscriptionFeeQuery() { PageNumber = pageNumber, PageSize = pageSize, SearchText = SearchText });
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteSubscriptionFeeCommand() { Id = id });
            return Ok(result);
        }

    }
}
