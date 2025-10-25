namespace FishSpinDays.Web.Helpers.Filters
{
    using FishSpinDays.Web.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    // Critical operations attribute
    [AttributeUsage(AttributeTargets.Method)]
    public class CriticalOperationAttribute : ActionFilterAttribute
    {
        public string OperationName { get; set; }
        public int SlowThresholdMs { get; set; } = 1000;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PublicationsAPIController>>();
            var operationName = OperationName ?? context.ActionDescriptor.DisplayName;

            using var scope = logger.BeginScope("Critical Operation: {Operation}", operationName);
            using var activity = new Activity(operationName);
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting critical API operation: {Operation}", operationName);

            try
            {
                var result = await next();

                stopwatch.Stop();
                logger.LogInformation("Critical operation completed: {Operation}, Duration: {Duration}ms, Success: {Success}",
                    operationName, stopwatch.ElapsedMilliseconds, result.Exception == null);

                if (stopwatch.ElapsedMilliseconds > SlowThresholdMs)
                {
                    logger.LogWarning("Slow critical operation detected: {Operation} took {Duration}ms", operationName, stopwatch.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Critical operation failed: {Operation}, Duration: {Duration}ms", operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }
    }
}
