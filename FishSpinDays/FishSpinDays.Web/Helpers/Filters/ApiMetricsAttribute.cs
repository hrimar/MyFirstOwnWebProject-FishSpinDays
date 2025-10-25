namespace FishSpinDays.Web.Helpers.Filters
{
    using FishSpinDays.Web.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Method)]
    public class ApiMetricsAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PublicationsAPIController>>();

            var endpoint = context.ActionDescriptor.DisplayName;
            var method = context.HttpContext.Request.Method;
            var requestSize = context.HttpContext.Request.ContentLength ?? 0;
            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();

            using var scope = logger.BeginScope("API Metrics - Endpoint: {Endpoint}", endpoint);

            var stopwatch = Stopwatch.StartNew();

            logger.LogInformation("API Request - Endpoint: {Endpoint}, Method: {Method}, RequestSize: {RequestSize} bytes, UserAgent: {UserAgent}",
                endpoint, method, requestSize, userAgent);

            var result = await next();
            stopwatch.Stop();

            var statusCode = context.HttpContext.Response.StatusCode;
            var success = result.Exception == null && statusCode >= 200 && statusCode < 300;

            logger.LogInformation("API Response - Endpoint: {Endpoint}, StatusCode: {StatusCode}, Duration: {Duration}ms, Success: {Success}",
                endpoint, statusCode, stopwatch.ElapsedMilliseconds, success);

            // Log slow API calls
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                logger.LogWarning("Slow API call detected - Endpoint: {Endpoint} took {Duration}ms",
                    endpoint, stopwatch.ElapsedMilliseconds);
            }

            // Log error rates for monitoring
            if (!success)
            {
                logger.LogWarning("API call failed - Endpoint: {Endpoint}, StatusCode: {StatusCode}, Duration: {Duration}ms",
                    endpoint, statusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
