namespace FishSpinDays.Web.Helpers.Filters
{
    using FishSpinDays.Web.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    // Simple operations attribute
    [AttributeUsage(AttributeTargets.Method)]
    public class SimpleOperationAttribute : ActionFilterAttribute
    {
        public string OperationName { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<PublicationsAPIController>>();
            var operationName = OperationName ?? context.ActionDescriptor.DisplayName;

            logger.LogDebug("Simple API operation: {Operation}", operationName);

            try
            {
                await next();
                logger.LogInformation("Simple operation completed: {Operation}", operationName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Simple operation failed: {Operation}", operationName);
                throw;
            }
        }
    }
}
