using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FishSpinDays.Common.Identity.BindingModels;
using FishSpinDays.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace FishSpinDays.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthController> logger;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [AllowAnonymous]
        [HttpPost("")]          // api/Auth
        public async Task<IActionResult> GenerateToken(UserBindingModel model)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.FirstOrDefault();

            using var scope = logger.BeginScope("Authentication - Username: {Username}, IP: {IpAddress}", model?.Username, ipAddress);

            try
            {
                if (!ModelState.IsValid)
                {
                    logger.LogWarning("Invalid authentication request - Username: {Username}, IP: {IpAddress}", model?.Username, ipAddress);
                    return BadRequest(ModelState);
                }

                logger.LogInformation("Authentication attempt - Username: {Username}, IP: {IpAddress}, UserAgent: {UserAgent}", 
                    model.Username, ipAddress, userAgent);

                var user = await this.userManager.FindByNameAsync(model.Username);
                
                if (user == null)
                {
                    logger.LogWarning("Authentication failed - User not found: {Username}, IP: {IpAddress}", model.Username, ipAddress);
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                bool isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
                
                if (!isPasswordValid)
                {
                    logger.LogWarning("Authentication failed - Invalid password: {Username}, IP: {IpAddress}", model.Username, ipAddress);
                    return Unauthorized(new { Message = "Invalid credentials." });
                }

                var roles = await this.userManager.GetRolesAsync(user);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetSection("TokenValidationParameter").Value));
                var token = new JwtSecurityToken(
                    claims: new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, string.Join(", ", roles))
                    },
                    issuer: "localhost",
                    audience: "localhost",
                    expires: DateTime.Now + TimeSpan.FromHours(24),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                logger.LogInformation("Authentication successful - User: {Username}, IP: {IpAddress}, TokenExpires: {TokenExpires}", 
                    model.Username, ipAddress, DateTime.Now.AddHours(24));

                return Ok(new { Token = tokenString });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Authentication error - Username: {Username}, IP: {IpAddress}", model?.Username, ipAddress);
                return StatusCode(500, new { Message = "An error occurred during authentication." });
            }
        }
    }
}