namespace FishSpinDays.Web.Helpers.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    using System.Text;

    [AttributeUsage(AttributeTargets.Method)]
    public class ApiSecurityLoggingAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiSecurityLoggingAttribute>>();

            var user = context.HttpContext.User;
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
            var userId = user.Identity?.Name;
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var endpoint = context.ActionDescriptor.DisplayName;

            // Privacy-compliant IP logging - hash for anonymization
            var ipHash = HashIpAddress(ipAddress);

            using var scope = logger.BeginScope("API Security - Endpoint: {Endpoint}, User: {UserId}, IPHash: {IpHash}, Authenticated: {IsAuthenticated}",
                endpoint, userId, ipHash, isAuthenticated);

            if (isAuthenticated)
            {
                logger.LogInformation("Authenticated API access - User: {UserId}, IPHash: {IpHash}, Endpoint: {Endpoint}", 
                    userId, ipHash, endpoint);
            }
            else
            {
                // For unauthenticated access, we may need IP for security but hash it
                logger.LogWarning("Unauthenticated API access attempt - IPHash: {IpHash}, Endpoint: {Endpoint}", 
                    ipHash, endpoint);
            }

            var result = await next();

            // Log potential security issues
            if (result.Exception != null)
            {
                logger.LogWarning("API security exception - User: {UserId}, IPHash: {IpHash}, Endpoint: {Endpoint}, Exception: {ExceptionType}",
                    userId, ipHash, endpoint, result.Exception.GetType().Name);
            }

            var statusCode = context.HttpContext.Response.StatusCode;
            if (statusCode == 401 || statusCode == 403)
            {
                logger.LogWarning("API access denied - User: {UserId}, IPHash: {IpHash}, Endpoint: {Endpoint}, StatusCode: {StatusCode}",
                    userId, ipHash, endpoint, statusCode);
            }
        }

        private static string HashIpAddress(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return "unknown";

            // Use SHA256 hash for anonymization while keeping ability to correlate requests
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(ipAddress + "salt")); // Add salt for security
            return Convert.ToBase64String(hash)[..8]; // First 8 chars for correlation
        }
    }
}
