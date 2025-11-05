namespace FishSpinDays.Web.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class ApiErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ApiErrorHandlingMiddleware> logger;

        public ApiErrorHandlingMiddleware(RequestDelegate next, ILogger<ApiErrorHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);

                // Handle API 404 responses
                if (context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode == 404)
                {
                    await HandleApiErrorResponseAsync(context, HttpStatusCode.NotFound, "Endpoint not found");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred for API request: {Path}", context.Request.Path);

                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    await HandleApiErrorResponseAsync(context, HttpStatusCode.InternalServerError, "An internal server error occurred");
                }
                else
                {
                    throw; // Let normal error handling take care of web pages
                }
            }
        }

        private async Task HandleApiErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = message,
                statusCode = (int)statusCode,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}