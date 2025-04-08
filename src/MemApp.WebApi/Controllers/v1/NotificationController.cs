using MediatR;
using MemApp.Application.Com.Commands.CreateFeedback;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotificationController : ApiControllerBase
    {
        public readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, ISender sender) : base(sender)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotification()
        {
            var result = await _notificationService.GetAllNotification();
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetUnreadNotification()
        {
            var result = await _notificationService.GetUnreadNotification();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNotificationStatus(int[] unreadNotificationIds)
        {
            var result = await _notificationService.UpdateNotificationStatus(unreadNotificationIds);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateNotificationEmail(NotificationEmailDto notificationEmail)
        {
            var result = await _notificationService.SaveOrUpdateNotificationEmail(notificationEmail);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationEmail()
        {
            var result = await _notificationService.GetNotificationEmail();
            return Ok(result);
        }


    }
}
