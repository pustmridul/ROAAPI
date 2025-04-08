using MemApp.Application.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace MemApp.WebApi.Middlewares
{

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            switch (exception)
            {
                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = badRequestException.Message;
                    break;
                case LoginFailedException loginFailedException:
                    code = HttpStatusCode.Forbidden;
                    result = loginFailedException.Message;
                    break;
                case  UnauthorizedAccessException unauthorizedAccessException :
                    code = HttpStatusCode.Unauthorized;
                    result = unauthorizedAccessException.Message;
                    break;
                case DataValidationException validationException:
                    code = HttpStatusCode.NotAcceptable;

                    foreach (var f in validationException.Failures)
                    {
                        foreach (var s in f.Value)
                        {
                            result += s + ";";
                        }

                    }
                    result = JsonConvert.SerializeObject(validationException.Failures);
                    break;
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty)

            {
                result = exception.Message;
                BuildException(exception.InnerException, result);
                result = JsonConvert.SerializeObject(new
                {
                    error = exception.Message,
                    exception = JsonConvert.SerializeObject(exception)
                });
            }

            return context.Response.WriteAsync(result);
        }

        void BuildException(Exception? ex, string message)
        {
            if (ex?.InnerException != null)
            {
                message += ex.Message;
                BuildException(ex.InnerException, message);
            }
        }
    }




    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }



    //public class CustomExceptionMiddleware
    //{
    //    private readonly RequestDelegate _next;

    //    public CustomExceptionMiddleware(RequestDelegate next)
    //    {
    //        _next = next;
    //    }
    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        try
    //        {
    //            await _next(context); // Call the next middleware
    //        }
    //        catch (Exception ex)
    //        {
    //            await HandleExceptionAsync(context, ex);
    //        }
    //    }


    //    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    //    {
    //        // Customize your exception handling logic here
    //        context.Response.ContentType = "application/json";
    //        if(exception.Message== "Attempted to perform an unauthorized operation.")
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //        }
    //        else
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //        }


    //        // Create a response object or message
    //        var responseMessage = new
    //        {
    //            StatusCode = context.Response.StatusCode,
    //            Message = "An error occurred while processing your request."
    //            // You can also include additional error details if needed
    //        };

    //        return context.Response.WriteAsync(responseMessage.ToString());
    //    }
    //}
}