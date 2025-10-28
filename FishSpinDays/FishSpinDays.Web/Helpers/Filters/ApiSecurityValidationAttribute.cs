namespace FishSpinDays.Web.Helpers.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Additional security validation filter for API endpoints
    /// Validates request headers and provides additional security checks
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiSecurityValidationAttribute : ActionFilterAttribute
    {
        public bool RequireSecureConnection { get; set; } = true;
        public bool ValidateContentType { get; set; } = true;
        public bool CheckUserAgent { get; set; } = true;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiSecurityValidationAttribute>>();

            if (!await ValidateSecurityRequirements(context, logger))
            {
                return; // Response has been set by validation
            }

            await next();
        }

        private async Task<bool> ValidateSecurityRequirements(ActionExecutingContext context, ILogger logger)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            // Check for HTTPS in production
            if (RequireSecureConnection && !request.IsHttps)
            {
                var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
                if (isProduction)
                {
                    logger.LogWarning("API request rejected - HTTPS required in production. Path: {Path}", request.Path);
                    context.Result = new StatusCodeResult(426); // Upgrade Required
                    return false;
                }
            }

            // Validate Content-Type for POST/PUT requests
            if (ValidateContentType && (request.Method == "POST" || request.Method == "PUT"))
            {
                var contentType = request.ContentType;
                if (string.IsNullOrEmpty(contentType) || (!contentType.Contains("application/json") && !contentType.Contains("application/x-www-form-urlencoded")))
                {
                    logger.LogWarning("API request rejected - Invalid Content-Type: {ContentType}. Path: {Path}", contentType, request.Path);
                    context.Result = new BadRequestObjectResult(new { Message = "Invalid Content-Type" });
                    return false;
                }
            }

            // Check for suspicious User-Agent patterns
            if (CheckUserAgent)
            {
                var userAgent = request.Headers.UserAgent.FirstOrDefault();
                if (IsSuspiciousUserAgent(userAgent))
                {
                    logger.LogWarning("API request rejected - Suspicious User-Agent: {UserAgent}. Path: {Path}", userAgent, request.Path);
                    context.Result = new StatusCodeResult(403); // Forbidden
                    return false;
                }
            }

            // Add security response headers
            AddSecurityResponseHeaders(response);

            return true;
        }

        private static bool IsSuspiciousUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return true; // Reject empty User-Agent
            }

            // Check for common attack patterns
            var suspiciousPatterns = new[]
          {
       "sqlmap",
      "nikto",
        "nmap",
                "masscan",
  "wget",
       "curl", // Be cautious with curl - might be legitimate
    "python-requests",
    "bot", // Generic bot pattern
       "crawler",
     "spider"
    };

            var lowerUserAgent = userAgent.ToLowerInvariant();
            return suspiciousPatterns.Any(pattern => lowerUserAgent.Contains(pattern));
        }

        private static void AddSecurityResponseHeaders(Microsoft.AspNetCore.Http.HttpResponse response)
        {
            // Add API-specific security headers
            if (!response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                response.Headers["X-Content-Type-Options"] = "nosniff";
            }

            if (!response.Headers.ContainsKey("X-Frame-Options"))
            {
                response.Headers["X-Frame-Options"] = "DENY";
            }

            if (!response.Headers.ContainsKey("Cache-Control"))
            {
                response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            }

            if (!response.Headers.ContainsKey("Pragma"))
            {
                response.Headers["Pragma"] = "no-cache";
            }
        }
    }
}