using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.App.Queries.MemberInfomation;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.app
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberInformationsController : ApiControllerBase
    {
        public MemberInformationsController(ISender sender) : base(sender) { }
      
        [HttpGet]
        public async Task<MemberInfoListVm> GetMInfoByMShipNo(string MemberShipNo)
        {
            return await Mediator.Send(new GetMemberListByMShipQuery() { MemberShipNo = MemberShipNo });
        }

        [HttpGet]
        public async Task<MemberInfoListVm> GetMInfoByCardNo(string cardNo)
        {
            return await Mediator.Send(new GetMemberListByCardNoQuery() { CardNo = cardNo });
        }
    }
}
