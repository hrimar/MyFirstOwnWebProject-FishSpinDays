namespace FishSpinDays.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using FishSpinDays.Common.Identity.BindingModels;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;
    using System.Linq;

    [Route("api/publications")] // for url: http://localhost:51034/api/publications
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PublicationsAPIController : BaseAPIController
    {
        private readonly UserManager<User> userManager;
        private readonly IIdentityService identityService;
        private readonly ILogger<PublicationsAPIController> logger;

        public PublicationsAPIController(
            UserManager<User> userManager,
            IIdentityService identityService, 
            IBasePublicationsService baseService,
            ILogger<PublicationsAPIController> logger)
            : base(baseService)
        {
            this.userManager = userManager;
            this.identityService = identityService;
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================================
        // CRITICAL OPERATIONS - Full Logging with Performance Monitoring
        // ============================================================================

        [HttpPost("")]
        [Authorize]  // To use this for API makes AuthControler
        public async Task<IActionResult> CreatePublication([FromBody]PublicationBindingModel model, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("CreatePublication - Title: {Title}, Section: {Section}, UserId: {UserId}", model?.Title, model?.Section, User?.Identity?.Name);
            using var activity = new Activity("CreatePublication");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting API CreatePublication operation with cancellation support");

            try
            {
                if (!this.ModelState.IsValid)
                {
                    logger.LogWarning("Invalid model state for CreatePublication - Title: {Title}", model?.Title);
                    return BadRequest(this.ModelState);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("CreatePublication was cancelled before processing - Title: {Title}", model?.Title);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Getting user and section for publication creation");
                User author = await this.userManager.GetUserAsync(this.User);
                var section = await this.identityService.GetSectionByNameAsync(model.Section, cancellationToken);

                if (author == null || section == null)
                {
                    logger.LogWarning("User or section not found - AuthorFound: {AuthorFound}, SectionFound: {SectionFound}, Section: {SectionName}", 
                        author != null, section != null, model.Section);
                    return NotFound(new { Message = "User or section not found." });
                }

                logger.LogDebug("Creating publication via IdentityService");
                var publication = await this.identityService.CreatePublicationAsync(author, section, model.Title, model.Description, cancellationToken);

                if (publication == null)
                {
                    logger.LogError("Failed to create publication - returned null from service - Title: {Title}, AuthorId: {AuthorId}", model.Title, author.Id);
                    return StatusCode(500, new { Message = "Failed to create publication. Please try again." });
                }

                stopwatch.Stop();
                logger.LogInformation("Successfully created publication via API - ID: {PublicationId}, Title: {Title}, AuthorId: {AuthorId}, Duration: {ElapsedMs}ms", 
                    publication.Id, model.Title, author.Id, stopwatch.ElapsedMilliseconds);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Slow CreatePublication API operation - took {ElapsedMs}ms for title: {Title}", stopwatch.ElapsedMilliseconds, model.Title);
                }

                return CreatedAtAction("Details", new { id = publication.Id });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("CreatePublication API was cancelled after {ElapsedMs}ms - Title: {Title}", stopwatch.ElapsedMilliseconds, model?.Title);
                return StatusCode(499, new { Message = "Request was cancelled." }); // Client Closed Request
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error creating publication via API after {ElapsedMs}ms - Title: {Title}, Section: {Section}", 
                    stopwatch.ElapsedMilliseconds, model?.Title, model?.Section);
                return StatusCode(500, new { Message = "An error occurred while creating the publication." });
            }
            finally
            {
                activity.Stop();
            }
        }

        // ============================================================================
        // IMPORTANT OPERATIONS - Moderate Logging
        // ============================================================================

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<ActionResult<PartPublicationsViewModel>> GetAllPublications(int? id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetAllPublications API - Page: {Page}", id?.ToString() ?? "null");
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting GetAllPublications API operation");

            try
            {
                if (!id.HasValue)
                {
                    id = WebConstants.DefaultPage;
                    logger.LogDebug("Using default page for API request: {DefaultPage}", id);
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);
                var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, 3, cancellationToken);

                // Handle empty results gracefully
                if (publications == null || !publications.Any())
                {
                    logger.LogWarning("No publications found for API request - Page: {Page}", id);
                    return new PartPublicationsViewModel()
                    {
                        Id = id.Value,
                        Count = 0,
                        Publications = Enumerable.Empty<PublicationShortViewModel>()
                    };
                }

                var viewModel = new PartPublicationsViewModel()
                {
                    Id = id.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications
                };

                stopwatch.Stop();
                logger.LogInformation("GetAllPublications API completed - Page: {Page}, Count: {ResultCount}, Duration: {ElapsedMs}ms", 
                    id, publications.Count(), stopwatch.ElapsedMilliseconds);

                return viewModel;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetAllPublications API was cancelled after {ElapsedMs}ms - Page: {Page}", stopwatch.ElapsedMilliseconds, id);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetAllPublications API after {ElapsedMs}ms - Page: {Page}", stopwatch.ElapsedMilliseconds, id);
                return StatusCode(500, new { Message = "An error occurred while retrieving publications." });
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Minimal Logging
        // ============================================================================

        [HttpGet("{id}", Name = "Details")] // Details will be the name of this method and with this name will be used on row.99!
        [AllowAnonymous]
        public async Task<IActionResult> GetPublication(int id, CancellationToken cancellationToken = default) 
        {
            logger.LogDebug("Getting single publication via API - ID: {PublicationId}", id);

            try
            {
                var publicationModel = await this.BaseService.GetPublicationAsync(id, cancellationToken);
                
                if (publicationModel == null)
                {
                    logger.LogInformation("Publication not found via API - ID: {PublicationId}", id);
                    return NotFound(new { Message = "The publication does not exist." });
                }

                logger.LogInformation("Publication found via API - ID: {PublicationId}, Title: {Title}", id, publicationModel.Title);

                return Ok(publicationModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetPublication API cancelled - ID: {PublicationId}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting publication via API - ID: {PublicationId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving the publication." });
            }
        }

        // ============================================================================
        // PRIVATE HELPER METHODS
        // ============================================================================

        private async Task<int> ArrangePagesCountAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Calculating API page count");
            
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(cancellationToken);
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            logger.LogDebug("API page count calculation - Total publications: {TotalPublications}, Total pages: {TotalPages}", 
                totalPublicationsCount, requiredPagesForThisPublications);

            return requiredPagesForThisPublications;
        }
    }
}
