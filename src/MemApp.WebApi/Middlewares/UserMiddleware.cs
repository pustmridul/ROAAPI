using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace MemApp.WebApi.Middlewares
{
    public class UserMiddleware
    {
        
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Access user information from the claims
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Do something with the user ID, for example, log it
            Console.WriteLine($"User ID: {userId}");

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
