using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.MessageInboxs.Commands.MessageUpdate;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.MessageInboxs.Queries.GetAllMessages;
using MemApp.Application.MessageInboxs.Queries.GetMessageByMemberId;
using MemApp.Application.MessageInboxs.Queries.GetMessageByType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemApp.Application.MessageInboxs.Queries;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessageInboxsController : ApiControllerBase
    {
        public MessageInboxsController(ISender mediator) : base(mediator) { }
   
        [HttpPost]
        public async Task<IActionResult> Save(int id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new MessageUpdateCommand { Id = id });
            return result.HasError ? BadRequest(result) : Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageParams pageParams)
        {
            if(pageParams is null)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(new GetAllMessagesQuery() { PageNo = pageParams.PageNo, PageSize = pageParams.PageSize }));
        }
 
        [HttpGet]
        public async Task<IActionResult> GetByMemberId([FromQuery] MessageInboxQueryDto dto)
        {
            return  Ok(await Mediator.Send(new GetMessageByMemberIdQueries() 
            { 
                Model = dto
            }));
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalReadUnReadMessages(int memberId, bool isRead)
        {
            return Ok(await Mediator.Send(new GetTotalReadUnReadMessagesQueries()
            {
                MemberId = memberId,
                IsRead = isRead
            }));
        }

        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetMessageByType(string type)
        {

            return Ok(await Mediator.Send(new GetMessageByTypeQueries() { MessageType = type }));
        }

    }
}
