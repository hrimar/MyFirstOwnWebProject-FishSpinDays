namespace FishSpinDays.Web.Extensions
{
    using FishSpinDays.Web.Configuration;
    using FishSpinDays.Web.Middleware;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    /// <summary>
    /// Extension methods for configuring security features
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Adds comprehensive security services to the DI container
        /// </summary>
        public static IServiceCollection AddEnhancedSecurity(this IServiceCollection services)
        {
            // Configure HSTS
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365); // 1 year
                options.ExcludedHosts.Clear(); // Don't exclude any hosts by default
            });

            // Configure HTTPS redirection
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443; // Standard HTTPS port
            });

            // Note: Anti-forgery is already configured via AutoValidateAntiforgeryTokenAttribute in Startup.cs

            return services;
        }

        /// <summary>
        /// Adds security headers middleware with default configuration
        /// </summary>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseSecurityHeaders(new SecurityHeadersOptions());
        }

        /// <summary>
        /// Adds security headers middleware with custom configuration
        /// </summary>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, SecurityHeadersOptions options)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>(options);
        }

        /// <summary>
        /// Adds security headers middleware configured for a Razor Pages application
        /// </summary>
        public static IApplicationBuilder UseSecurityHeadersForRazorPages(this IApplicationBuilder app)
        {
            var options = new SecurityHeadersOptions
            {
                FrameOptions = "SAMEORIGIN", // Allow framing from same origin for potential admin panels
                ReferrerPolicy = "strict-origin-when-cross-origin",
                ContentSecurityPolicy = BuildRazorPagesCSP(),
                PermissionsPolicy = "camera=(), microphone=(), geolocation=(), payment=(), usb=()"
            };

            return app.UseSecurityHeaders(options);
        }

        /// <summary>
        /// Builds a Content Security Policy suitable for Razor Pages applications
        /// </summary>
        private static string BuildRazorPagesCSP()
        {
            return string.Join("; ", new[]
           {
    "default-src 'self'", // Only allow resources from same origin by default
  "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com", // Allow inline scripts and popular CDNs
 "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com", // Allow inline styles and CDNs
         "img-src 'self' data: https: http:", // Allow images from HTTPS and data URIs
      "font-src 'self' https://fonts.gstatic.com https://cdn.jsdelivr.net", // Allow fonts from Google Fonts and CDNs
     "connect-src 'self'", // Allow AJAX requests to same origin only
 "frame-src 'none'", // Prevent embedding in frames
       "object-src 'none'", // Prevent plugins like Flash
             "base-uri 'self'", // Restrict base URI
    "form-action 'self'", // Only allow forms to submit to same origin
       "upgrade-insecure-requests" // Automatically upgrade HTTP requests to HTTPS
   });
        }
    }
}