namespace MemApp.WebApi.Middlewares
{
    public class CorsMiddleware
    {

        private readonly RequestDelegate _next;
        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,X-CSRF-Token,X-Requested-with,Accept,Accept-Version,Content-Length,Content-MD5,Date,X-Api-Version,X-File-Name");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");
            return _next(context);
        }
    }
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
