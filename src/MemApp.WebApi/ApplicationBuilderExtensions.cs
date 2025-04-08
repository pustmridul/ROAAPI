using Microsoft.AspNetCore.Builder;

namespace MemAppWebApi
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}