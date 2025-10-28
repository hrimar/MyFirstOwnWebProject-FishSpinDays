namespace FishSpinDays.Web.Configuration
{
    /// <summary>
    /// Configuration options for security headers
    /// </summary>
    public class SecurityHeadersOptions
    {
        /// <summary>
        /// X-Frame-Options header value (DENY, SAMEORIGIN, or ALLOW-FROM uri)
        /// </summary>
        public string FrameOptions { get; set; } = "SAMEORIGIN";

        /// <summary>
        /// Referrer-Policy header value
        /// </summary>
        public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";

        /// <summary>
        /// Content-Security-Policy header value
        /// </summary>
        public string ContentSecurityPolicy { get; set; } = string.Empty;

        /// <summary>
        /// Permissions-Policy header value (formerly Feature-Policy)
        /// </summary>
        public string PermissionsPolicy { get; set; } = "camera=(), microphone=(), geolocation=()";
    }
}