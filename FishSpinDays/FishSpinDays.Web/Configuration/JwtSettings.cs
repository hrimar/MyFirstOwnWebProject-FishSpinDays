namespace FishSpinDays.Web.Configuration
{
    /// <summary>
    /// JWT configuration settings
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        /// <summary>
        /// Token issuer (who created the token)
        /// </summary>
        public string Issuer { get; set; } = "localhost";

        /// <summary>
        /// Token audience (who the token is intended for)
        /// </summary>
        public string Audience { get; set; } = "localhost";

        /// <summary>
        /// Signing key for token validation - if empty, falls back to TokenValidationParameter
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in minutes
        /// </summary>
        public int ExpirationMinutes { get; set; } = 1440; // 24 hours

        /// <summary>
        /// Require HTTPS metadata endpoint
        /// </summary>
        public bool RequireHttpsMetadata { get; set; } = true;

        /// <summary>
        /// Save token in authentication properties
        /// </summary>
        public bool SaveToken { get; set; } = false;

        /// <summary>
        /// Clock skew tolerance in minutes
        /// </summary>
        public int ClockSkew { get; set; } = 5;

        /// <summary>
        /// Validate issuer
        /// </summary>
        public bool ValidateIssuer { get; set; } = true;

        /// <summary>
        /// Validate audience
        /// </summary>
        public bool ValidateAudience { get; set; } = true;

        /// <summary>
        /// Validate lifetime
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// Validate issuer signing key
        /// </summary>
        public bool ValidateIssuerSigningKey { get; set; } = true;
    }
}