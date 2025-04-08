using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using ResApp.Application.Com.Commands.ROAPayment;
using ResApp.Application.Com.Commands.RoaMembershipFee;
using ResApp.Application.Com.Commands.RoaMembershipFee.Models;
using ResApp.Application.Com.Commands.ROASubscription.Queries;
using ResApp.Application.Com.Commands.RoaMembershipFee.Queries;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoMembershipFeeController : ApiNewControllerBase
    {
        public RoMembershipFeeController(ISender mediator) : base(mediator)
        {
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Payment(MembershipFeePayReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new RoMembershipFeePaymentCommand() { Model = model });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPaidByMemberId(int MemberId)
        {

            var result = await Mediator.Send(new GetPaidByROMemberIdQuery() { MemberId = MemberId });
            return Ok(result);
        }
    }
}
