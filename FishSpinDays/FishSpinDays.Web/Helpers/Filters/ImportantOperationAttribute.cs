namespace FishSpinDays.Web.Helpers.Filters
{
    using FishSpinDays.Web.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    // Important operations attribute  
    [AttributeUsage(AttributeTargets.Method)]
    public class ImportantOperationAttribute : ActionFilterAttribute
    {
        public string OperationName { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PublicationsAPIController>>();
            var operationName = OperationName ?? context.ActionDescriptor.DisplayName;

            using var scope = logger.BeginScope("Important Operation: {Operation}", operationName);
            var stopwatch = Stopwatch.StartNew();

            logger.LogDebug("Starting important API operation: {Operation}", operationName);

            try
            {
                var result = await next();
                stopwatch.Stop();

                logger.LogInformation("Important operation completed: {Operation}, Duration: {Duration}ms", operationName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Important operation failed: {Operation}, Duration: {Duration}ms", operationName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
