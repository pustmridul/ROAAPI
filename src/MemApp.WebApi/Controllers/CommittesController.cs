using MediatR;
using MemApp.Application.Mem.MemberTypes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Res.Domain.Entities;
using ResApp.Application.Com.Commands.MemberRegistration;
using ResApp.Application.Com.Commands.UpdateThana;
using ResApp.Application.Com.Queries.GetMemberRegistrationInfo;
using ResApp.Application.Models.DTOs;
using ResApp.Application.ROA.CommitteeCategory.Command;
using ResApp.Application.ROA.CommitteeCategory.Queries;
using ResApp.Application.ROA.Committees.Commands;
using ResApp.Application.ROA.Committees.Models;
using ResApp.Application.ROA.Committees.Queries;
using ResApp.Application.ROA.MemberRegistration;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommitteesController : ApiNewControllerBase
    {
       
        public CommitteesController( ISender sender) : base(sender)
        {
           
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommitteeSearchParam paginationParams)
        {
            var result = await Mediator.Send(new GetAllRoCommitteeQuery() 
            { 
               Model=paginationParams
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] RoCommitteeReq ExecutiveCommittee)
        {
            // command.WebRootPath = _hostingEnv.WebRootPath;
            var result = await Mediator.Send(new CreateRoCommitteeCommand()
            {
                Model = ExecutiveCommittee
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMemberInfoByMemberShip(string memberShipNo)
        {
            return Ok(await Mediator.Send(new GetAllMemberByMemberShipNoQuery()
            {
                MemberShipNo = memberShipNo
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(int Id)
        {
            return Ok(await Mediator.Send(new GetRoCommitteeByIdQuery()
            {
                Id = Id
            }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveCategory(CreateCommitteeCatCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            return Ok(await Mediator.Send(new GetAllCommitteeCatQuery()
            {
               
            }));

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            return Ok(await Mediator.Send(new GetCommitteeCatByIdQuery()
            {
                CategoryId = categoryId
            }));

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var result = await Mediator.Send(new DeleteCommitteeCatCommand() { Id = id });
            return Ok(result);
        }
    }
}
