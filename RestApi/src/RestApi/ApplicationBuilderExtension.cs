using System;
using Microsoft.AspNetCore.Builder;
using RestApi.Middleware;

namespace RestApi
{
    public static class ApplicationBuilderExtension
    {
        public static void UseCorsWithStandardMethods(this IApplicationBuilder app)
        {
            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods("PUT", "POST", "DELETE", "OPTIONS", "HEAD", "GET", "PATCH")
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(3600))
                );
        }

        public static void UseLoggerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}