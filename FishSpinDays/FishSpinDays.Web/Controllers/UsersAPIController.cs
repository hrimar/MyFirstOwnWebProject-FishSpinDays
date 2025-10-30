namespace FishSpinDays.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using FishSpinDays.Web.Helpers.Filters;
    using System.Linq;
    using FishSpinDays.Common.API.Models.Users;

    /// <summary>
    /// API Controller for user management
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiSecurityValidation]
    public class UsersAPIController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IIdentityService identityService;
        private readonly ILogger<UsersAPIController> logger;

        public UsersAPIController(
            UserManager<User> userManager,
            IIdentityService identityService,
            ILogger<UsersAPIController> logger)
        {
            this.userManager = userManager;
            this.identityService = identityService;
            this.logger = logger;
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        [SimpleOperation(OperationName = "GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { Message = "User not found." });
                }

                var roles = await userManager.GetRolesAsync(user);

                return Ok(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                    LockoutEnd = user.LockoutEnd,
                    IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
                });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting current user information");
                return StatusCode(500, new { Message = "An error occurred while retrieving user information." });
            }
        }

        /// <summary>
        /// Get user details by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetUser")]
        public async Task<IActionResult> GetUser(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await identityService.GetUserByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    PublicationsCount = user.Publications?.Count ?? 0,
                    CommentsCount = user.Comments?.Count ?? 0,
                    // Don't expose email or sensitive information in public endpoint
                });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user details - ID: {UserId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving user details." });
            }
        }

        /// <summary>
        /// Get user's publications
        /// </summary>
        [HttpGet("{id}/publications")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetUserPublications")]
        public async Task<IActionResult> GetUserPublications(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await identityService.GetUserByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                var publications = user.Publications?.Select(p => new
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreationDate = p.CreationDate,
                    Likes = p.Likes,
                    Section = p.Section?.Name,
                    CommentsCount = p.Comments?.Count ?? 0
                }).OrderByDescending(p => p.CreationDate) ?? Enumerable.Empty<object>();

                return Ok(publications);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user publications - ID: {UserId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving user publications." });
            }
        }

        /// <summary>
        /// Get user's comments
        /// </summary>
        [HttpGet("{id}/comments")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetUserComments")]
        public async Task<IActionResult> GetUserComments(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await identityService.GetUserByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                var comments = user.Comments?.Select(c => new
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreationDate = c.CreationDate,
                    Likes = c.Likes,
                    UnLikes = c.UnLikes,
                    PublicationId = c.PublicationId,
                    PublicationTitle = c.Publication?.Title
                }).OrderByDescending(c => c.CreationDate) ?? Enumerable.Empty<object>();

                return Ok(comments);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting user comments - ID: {UserId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving user comments." });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("me")]
        [SimpleOperation(OperationName = "UpdateCurrentUser")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { Message = "User not found." });
                }

                // Update allowed fields
                if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
                {
                    user.Email = model.Email;
                    user.EmailConfirmed = false; // Require email confirmation for new email
                }

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Message = "Profile updated successfully." });
                }

                return BadRequest(new { Message = "Failed to update profile.", Errors = result.Errors.Select(e => e.Description) });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { Message = "An error occurred while updating the profile." });
            }
        }
    }
}