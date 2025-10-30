using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FishSpinDays.Common.Identity.BindingModels;
using FishSpinDays.Models;
using FishSpinDays.Web.Configuration;
using FishSpinDays.Web.Helpers.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FishSpinDays.Web.Controllers.API
{
    [Route("api/[controller]")]
  [ApiController]
    [IgnoreAntiforgeryToken]
    [ApiSecurityValidation] // security validation
    public class AuthController : ControllerBase
    {
    private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
      private readonly ILogger<AuthController> logger;
        private readonly JwtSettings jwtSettings;

        public AuthController(
  UserManager<User> userManager, 
        IConfiguration configuration,
 ILogger<AuthController> logger)
        {
 this.userManager = userManager;
    this.configuration = configuration;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create JwtSettings directly from configuration
   this.jwtSettings = new JwtSettings();
      configuration.GetSection(JwtSettings.SectionName).Bind(this.jwtSettings);
      }

        [AllowAnonymous]
        [HttpPost("")]          // api/Auth
        public async Task<IActionResult> GenerateToken(UserBindingModel model)
        {
      var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
     var userAgent = Request.Headers.UserAgent.FirstOrDefault();

            using var scope = logger.BeginScope("Authentication - Username: {Username}, IPHash: {IPHash}", model?.Username, HashIpAddress(ipAddress));

            try
            {
          if (!ModelState.IsValid)
         {
        logger.LogWarning("Invalid authentication request - Username: {Username}", model?.Username);
         return BadRequest(ModelState);
                }

         logger.LogInformation("Authentication attempt - Username: {Username}, UserAgent: {UserAgent}", model.Username, userAgent);

      var user = await this.userManager.FindByNameAsync(model.Username);
         
    if (user == null)
          {
     logger.LogWarning("Authentication failed - User not found: {Username}", model.Username);
          return Unauthorized(new { Message = "Invalid credentials." });
  }

     bool isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
             
     if (!isPasswordValid)
          {
         logger.LogWarning("Authentication failed - Invalid password: {Username}", model.Username);
           return Unauthorized(new { Message = "Invalid credentials." });
        }

       var roles = await this.userManager.GetRolesAsync(user);

     // Generate JWT token using existing TokenValidationParameter or new JWT key
                var tokenString = GenerateJwtToken(user, roles);

    logger.LogInformation("Authentication successful - User: {Username}, TokenExpires: {TokenExpires}", 
             model.Username, DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes));

    return Ok(new { 
     Token = tokenString,
    ExpiresIn = jwtSettings.ExpirationMinutes * 60, // seconds
      TokenType = "Bearer"
});
     }
            catch (Exception ex)
       {
      logger.LogError(ex, "Authentication error - Username: {Username}", model?.Username);
        return StatusCode(500, new { Message = "An error occurred during authentication." });
            }
        }

   private string GenerateJwtToken(User user, IList<string> roles)
        {
            // Use JWT Key if configured, otherwise fallback to existing TokenValidationParameter
            var signingKey = !string.IsNullOrEmpty(jwtSettings.Key) 
    ? jwtSettings.Key 
      : configuration.GetSection("TokenValidationParameter").Value;

   var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
{
      new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Name, user.UserName),
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
   new Claim(JwtRegisteredClaimNames.Iat, 
        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
   ClaimValueTypes.Integer64)
            };

            // Add roles as separate claims (better for authorization)
   foreach (var role in roles)
            {
       claims.Add(new Claim(ClaimTypes.Role, role));
         }

      var token = new JwtSecurityToken(
         issuer: jwtSettings.Issuer,
      audience: jwtSettings.Audience,
claims: claims,
        expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
       signingCredentials: credentials);

          return new JwtSecurityTokenHandler().WriteToken(token);
 }

   private static string HashIpAddress(string ipAddress)
  {
         if (string.IsNullOrEmpty(ipAddress))
     return "unknown";

         // Privacy-compliant IP hashing
            using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(ipAddress + "fishspindays-salt"));
            return Convert.ToBase64String(hash)[..8]; // First 8 chars for correlation
      }
    }
}