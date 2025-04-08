using MediatR;
using MemApp.Application.com.Queries.GetAllEmergencyInfo;
using MemApp.Application.com.Queries.GetAllFeedback;
using MemApp.Application.Com.Commands.CreateFeedback;
using MemApp.Application.Com.Commands.CreateFeedbackCategory;
using MemApp.Application.Com.Commands.CreateReply;
using MemApp.Application.Com.Commands.RemoveFeedbackCategory;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries;
using MemApp.Application.Interfaces;
using MemApp.WebApi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.Controllers.v1.com
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedbacksController : ApiControllerBase
    {
        private INotificationService _notificationService;
        public FeedbacksController(INotificationService notificationService, ISender sender) : base(sender)
        {
            _notificationService = notificationService;
        }
     
        [HttpPost]
        public async Task<IActionResult> SaveFeedback(FeedbackReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateFeedbackCommand { Model = model });
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateFeedbackStatus(FeedbackStatusReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new UpdateFeedbackStatus { Model = model });
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveReply(ReplyReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateReplyCommand { Model = model });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFeedbackByMemberId(int memberId)
        {
            var result = await Mediator.Send(new GetFeedbackByMemberIdQuery() { MemberId = memberId });
            return Ok(result);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            var result = await Mediator.Send(new GetFeedbackIdQuery() { Id = id });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedback(int pageNo = 1, int pageSize = 1000, int FeedbackCategoryId = 0)
        {
            var result = await Mediator.Send(new GetAllFeedbackQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize,
                FeedbackCategoryId = FeedbackCategoryId
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveFeedbackCategory(FeedbackCategoryReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateFeedbackCategoryCommand { Model = model });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeedbackCategory(int pageNo = 1, int pageSize = 1000)
        {
            var result = await Mediator.Send(new GetAllFeedbackCategoryQuery()
            {
                PageNumber = pageNo,
                PageSize = pageSize
            });
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveFeedBackCategory(int id)
        {
            var result = await Mediator.Send(new RemoveFeedbackCategoryCommand() { Id = id });
            return Ok(result);
        }
    }
}
