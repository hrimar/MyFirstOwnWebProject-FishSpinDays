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
    using FishSpinDays.Common.API.Models.Comments;

    /// <summary>
    /// API Controller for managing comments
    /// </summary>
    [Route("api/comments")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiSecurityValidation]
    public class CommentsAPIController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IIdentityService identityService;
        private readonly ILogger<CommentsAPIController> logger;

        public CommentsAPIController(
            UserManager<User> userManager,
            IIdentityService identityService,
            ILogger<CommentsAPIController> logger)
        {
            this.userManager = userManager;
            this.identityService = identityService;
            this.logger = logger;
        }

        /// <summary>
        /// Add a new comment to a publication
        /// </summary>
        [HttpPost("")]
        [CriticalOperation(OperationName = "AddComment", SlowThresholdMs = 1000)]
        [ApiSecurityLogging]
        [ApiMetrics]
        public async Task<IActionResult> AddComment([FromBody] AddCommentModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await userManager.GetUserAsync(User);
                if (author == null)
                {
                    return Unauthorized(new { Message = "User not found." });
                }

                var publication = await identityService.GetPublicationByIdAsync(model.PublicationId, cancellationToken);
                if (publication == null)
                {
                    return NotFound(new { Message = "Publication not found." });
                }

                var comment = await identityService.AddCommentAsync(author, publication, model.Text, cancellationToken);
                if (comment == null)
                {
                    return StatusCode(500, new { Message = "Failed to add comment." });
                }

                return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, new
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    CreationDate = comment.CreationDate,
                    Author = author.UserName,
                    Likes = comment.Likes,
                    UnLikes = comment.UnLikes,
                    Message = "Comment added successfully."
                });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding comment - PublicationId: {PublicationId}", model?.PublicationId);
                return StatusCode(500, new { Message = "An error occurred while adding the comment." });
            }
        }

        /// <summary>
        /// Get a specific comment by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetComment")]
        public async Task<IActionResult> GetComment(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await identityService.GetCommentByIdAsync(id, cancellationToken);
                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found." });
                }

                return Ok(new
                {
                    Id = comment.Id,
                    Text = comment.Text,
                    CreationDate = comment.CreationDate,
                    Author = comment.Author?.UserName,
                    Likes = comment.Likes,
                    UnLikes = comment.UnLikes,
                    PublicationId = comment.PublicationId
                });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting comment - ID: {CommentId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving the comment." });
            }
        }

        /// <summary>
        /// Like a comment
        /// </summary>
        [HttpPost("{id}/like")]
        [SimpleOperation(OperationName = "LikeComment")]
        public async Task<IActionResult> LikeComment(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await identityService.GetCommentByIdAsync(id, cancellationToken);
                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found." });
                }

                var success = await identityService.IsLikedCommentAsync(comment, cancellationToken);
                if (success)
                {
                    return Ok(new { Message = "Comment liked successfully.", NewLikesCount = comment.Likes });
                }

                return StatusCode(500, new { Message = "Failed to like comment." });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error liking comment - ID: {CommentId}", id);
                return StatusCode(500, new { Message = "An error occurred while liking the comment." });
            }
        }

        /// <summary>
        /// Unlike a comment
        /// </summary>
        [HttpPost("{id}/unlike")]
        [SimpleOperation(OperationName = "UnlikeComment")]
        public async Task<IActionResult> UnlikeComment(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var comment = await identityService.GetCommentByIdAsync(id, cancellationToken);
                if (comment == null)
                {
                    return NotFound(new { Message = "Comment not found." });
                }

                var success = await identityService.IsUnLikedCommentAsync(comment, cancellationToken);
                if (success)
                {
                    return Ok(new { Message = "Comment unliked successfully.", NewUnlikesCount = comment.UnLikes });
                }

                return StatusCode(500, new { Message = "Failed to unlike comment." });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error unliking comment - ID: {CommentId}", id);
                return StatusCode(500, new { Message = "An error occurred while unliking the comment." });
            }
        }
    }
}