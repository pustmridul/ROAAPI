using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BroadcastsController : ApiControllerBase
    {
        private IBroadcastHandler _broadcastHandler;
        public BroadcastsController(IBroadcastHandler broadcastHandler, ISender sender) : base(sender)
        {
            _broadcastHandler = broadcastHandler;
        }

        [HttpPost]
        public async Task<IActionResult> SendWhatsAppMessage(string mobileNo)
        {
            var language = Request.Headers["language"].ToString();

            var result = await _broadcastHandler.SendMessage(mobileNo, language, "hello_world");

            if (!result)
                throw new Exception("Something went wrong!");

            return Ok("Sent successfully");
        }

        [HttpPost]
        public async Task<IActionResult> SendOTP(string mobileNo, string name)
        {
            var language = Request.Headers["language"].ToString();
            Random random = new();
            var otp = random.Next(0, 999999);

            var components = new List<WhatsAppComponent>
            {
                new WhatsAppComponent
                {
                    type = "body",
                    parameters = new List<object>
                    {
                        new { type = "text", text = name },
                        new { type = "text", text = otp.ToString("000000") },
                    }
                }
            };

            var result = await _broadcastHandler.SendMessage(mobileNo, language, "send_otp_new", components);

            if (!result)
                throw new Exception("Something went wrong!");

            return Ok("Sent successfully");
        }
    }
}
