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

namespace FishSpinDays.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("")]          // api/token
        public async Task<IActionResult> GenerateToken(UserBindingModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.Username);
            bool isPasswordValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (!isPasswordValid)
            {
                return Unauthorized();
            }

            var roles = await this.userManager.GetRolesAsync(user);

            var key = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(this.configuration.GetSection("TokenValidationParameter").Value));
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
            return Ok(new { Token = tokenString });
        }
    }
}