namespace FishSpinDays.Web.Middleware
{
    using FishSpinDays.Web.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    /// <summary>
    /// Middleware for adding security headers to HTTP responses
    /// Based on OWASP recommendations and Microsoft security best practices
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly SecurityHeadersOptions _options;

        public SecurityHeadersMiddleware(RequestDelegate next, ILogger<SecurityHeadersMiddleware> logger, SecurityHeadersOptions options)
        {
            _next = next;
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers before processing the request
            AddSecurityHeaders(context);

            await _next(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            var response = context.Response;

            try
            {
                // X-Content-Type-Options: Prevents MIME type sniffing
                if (!response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    response.Headers["X-Content-Type-Options"] = "nosniff";
                }

                // X-Frame-Options: Prevents clickjacking attacks
                if (!response.Headers.ContainsKey("X-Frame-Options") && !string.IsNullOrEmpty(_options.FrameOptions))
                {
                    response.Headers["X-Frame-Options"] = _options.FrameOptions;
                }

                // Referrer-Policy: Controls referrer information
                if (!response.Headers.ContainsKey("Referrer-Policy") && !string.IsNullOrEmpty(_options.ReferrerPolicy))
                {
                    response.Headers["Referrer-Policy"] = _options.ReferrerPolicy;
                }

                // Content-Security-Policy: Prevents XSS and other injection attacks
                if (!response.Headers.ContainsKey("Content-Security-Policy") && !string.IsNullOrEmpty(_options.ContentSecurityPolicy))
                {
                    response.Headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;
                }

                // X-XSS-Protection: Additional XSS protection (legacy but still useful)
                if (!response.Headers.ContainsKey("X-XSS-Protection"))
                {
                    response.Headers["X-XSS-Protection"] = "1; mode=block";
                }

                // Permissions-Policy: Controls browser features
                if (!response.Headers.ContainsKey("Permissions-Policy") && !string.IsNullOrEmpty(_options.PermissionsPolicy))
                {
                    response.Headers["Permissions-Policy"] = _options.PermissionsPolicy;
                }

                // Remove server information disclosure
                response.Headers.Remove("Server");
                response.Headers.Remove("X-Powered-By");
                response.Headers.Remove("X-AspNet-Version");
                response.Headers.Remove("X-AspNetMvc-Version");

                _logger.LogDebug("Security headers applied to response for {Path}", context.Request.Path);
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning(ex, "Failed to apply security headers for {Path}", context.Request.Path);
            }
        }
    }
}