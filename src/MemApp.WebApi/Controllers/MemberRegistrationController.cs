using MediatR;
using MemApp.Application.Com.Commands.CreateUser;
using MemApp.Application.Mem.BoardMeetingMinuets.Command;
using MemApp.Application.Mem.MemberTypes.Queries;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResApp.Application.Com.Commands.MemberRegistration;
using ResApp.Application.Com.Commands.MemberRegistration.UpdateMemberInfo;
using ResApp.Application.Com.Commands.UpdateThana;
using ResApp.Application.Com.Queries.GetMemberRegistrationInfo;
using ResApp.Application.Models;
using ResApp.Application.Models.DTOs;
using ResApp.Application.ROA.MemberCategory.Queries;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MemberRegistrationController : ApiNewControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnv;
        public MemberRegistrationController(ISender sender, IWebHostEnvironment hostingEnv) : base(sender)
        {
            _hostingEnv = hostingEnv;
        }

        //Member Part

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveMemberRegistration([FromForm] UpdateMemberRegCommand command)
        {
            command.WebRootPath= _hostingEnv.WebRootPath;
            return Ok(await Mediator.Send(command));
        }


        //Admin Part
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMemberRegistration([FromForm] CreateMemberRegCommand command)
        {
            command.WebRootPath = _hostingEnv.WebRootPath;
            return Ok(await Mediator.Send(command));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllMember([FromQuery] MemberSearchParam paginationParams)
         {
            return Ok(await Mediator.Send(new GetAllMemberRegInfoQuery()
            {
                //  UserId = userId
                //PageNo = paginationParams. PageNo,
                //PageSize =paginationParams.PageSize,
                //SearchText =paginationParams.SearchText,
                //AppId = paginationParams.AppId,
                Model=paginationParams
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPendingMember([FromQuery] MemberSearchParam paginationParams)
        {
            return Ok(await Mediator.Send(new GetPendingMemberRegInfoQuery()
            {
                //  UserId = userId
                //pageNo = PageNo,
                //pageSize = pageSize
                Model = paginationParams
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetApprovedMember([FromQuery] PaginationParams paginationParams)
        {
            return Ok(await Mediator.Send(new GetApprovedMemberRegInfoQuery()
            {
                //  UserId = userId
                PageNo = paginationParams.PageNo.GetValueOrDefault(),
                PageSize = paginationParams.PageSize.GetValueOrDefault(),
                AppId = paginationParams.AppId,
                SearchText = paginationParams.SearchText 
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMemberRegistrationInfo(int memberId)
        {
            return Ok(await Mediator.Send(new GetMemberRegInfoByIdQuery()
            {
                  MemberId = memberId
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMemberInfoByMemberShip(string memberShipNo)
        {
            return Ok(await Mediator.Send(new GetMemberByMemberShipNoQuery()
            {
                MemberShipNo = memberShipNo
            }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MakeMemberApprove([FromForm] ApproveMemberRegCommand command)
        {
            if(command==null) 
                throw new ArgumentNullException(nameof(command));
           
            return Ok(await Mediator.Send(command));
        }


        //Admin Part
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateMemberInfo([FromForm] UpdateMemberInfoCommand command)
        {
            command.WebRootPath = _hostingEnv.WebRootPath;
            return Ok(await Mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetTest()
        {
            return Ok("okay!");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberInfoCommand() { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllMemberCategory()
        {
            return Ok(await Mediator.Send(new GetAllRoMemberCatQuery()
            {
              
            }));
        }
    }
}
