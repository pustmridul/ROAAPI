using MediatR;
using MemApp.Application.Mem.Subscription.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscriptionModesController : ApiControllerBase
    {
        public SubscriptionModesController(ISender sender) : base(sender) { }

        [HttpGet]
        public async Task<IActionResult> GetSubscriptionModes()
        {
            var result = await Mediator.Send(new GetAllSubscriptionModeQuery());
            return Ok(result);
        }
    }
}
