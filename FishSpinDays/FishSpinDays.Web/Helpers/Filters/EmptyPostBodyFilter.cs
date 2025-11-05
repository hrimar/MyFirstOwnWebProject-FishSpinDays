namespace FishSpinDays.Web.Helpers.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Http;
    using System.Linq;

    /// <summary>
    /// Global action filter that handles POST requests without request body
    /// Prevents Content-Type validation errors for endpoints like like/unlike
    /// </summary>
    public class EmptyPostBodyFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // For POST requests without any [FromBody] or [FromForm] parameters
            if (context.HttpContext.Request.Method == "POST")
            {
                var hasBodyParameters = context.ActionDescriptor.Parameters
                     .Any(p => p.BindingInfo?.BindingSource != null &&
                        (p.BindingInfo.BindingSource.Id == "Body" || p.BindingInfo.BindingSource.Id == "Form"));

                // If no body parameters expected, set empty content type
                if (!hasBodyParameters &&
                (context.HttpContext.Request.ContentLength == 0 || context.HttpContext.Request.ContentLength == null))
                {
                    context.HttpContext.Request.ContentType = "application/x-www-form-urlencoded";
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }
}