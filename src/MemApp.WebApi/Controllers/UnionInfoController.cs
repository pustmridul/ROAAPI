using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.ROA.MunicipalityInfo.Command;
using ResApp.Application.ROA.MunicipalityInfo.Queries;
using ResApp.Application.ROA.Union.Command;
using ResApp.Application.ROA.Union.Queries;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnionInfoController : ApiNewControllerBase
    {
        public UnionInfoController(ISender sender) : base(sender) 
        {
        }
              

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveUnion(CreateUnionCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllUnionByThanaId(int ThanaId, int pageNo, int pageSize, string appId, string? searchText)
        {
            return Ok(await Mediator.Send(new GetUnionByThanaIdQuery()
            {
                ThanaId = ThanaId,
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText
            }));

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveUnion(int id)
        {
            var result = await Mediator.Send(new DeleteUnionCommand() { Id = id });
            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveMunicipality(CreateMunicipalityCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllMunicipalityByThanaId(int ThanaId, int pageNo, int pageSize, string appId, string? searchText)
        {
            return Ok(await Mediator.Send(new GetMuniByThanaIdQuery()
            {
                ThanaId = ThanaId,
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText
            }));

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveMunicipality(int id)
        {
            var result = await Mediator.Send(new DeleteMunicipalityCommand() { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveWard(CreateWardCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [HttpGet]
        public async Task<IActionResult> GetAllWardByThanaId(int? ThanaId,int? MunicipalId,int? UnionInfoId, int pageNo, int pageSize, string appId, string? searchText)
        {
            return Ok(await Mediator.Send(new GetWardByThanaQuery()
            {
                ThanaId = ThanaId,
                MunicipalityId = MunicipalId,
                PageNo = pageNo,
                PageSize = pageSize,
                AppId = appId,
                SearchText = searchText,
                UnionInfoId=UnionInfoId
            }));

        }


    }
}
