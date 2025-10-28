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
    using FishSpinDays.Web.Helpers.Filters;

    [Route("api/publications")] // for url: http://localhost:51034/api/publications
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiSecurityValidation] // security validation
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
            this.logger = logger;
        }

        // ============================================================================
        // CRITICAL OPERATIONS - Attribute-Based Logging
        // ============================================================================

        [HttpPost("")]
        [Authorize] // To use this for API makes AuthControler
        [CriticalOperation(OperationName = "CreatePublication", SlowThresholdMs = 1500)]
        [ApiSecurityLogging]
        [ApiMetrics]
        public async Task<IActionResult> CreatePublication([FromBody]PublicationBindingModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return BadRequest(this.ModelState);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                User author = await this.userManager.GetUserAsync(this.User);
                var section = await this.identityService.GetSectionByNameAsync(model.Section, cancellationToken);

                if (author == null || section == null)
                {
                    return NotFound(new { Message = "User or section not found." });
                }

                var publication = await this.identityService.CreatePublicationAsync(author, section, model.Title, model.Description, cancellationToken);

                if (publication == null)
                {
                    return StatusCode(500, new { Message = "Failed to create publication. Please try again." });
                }

                return CreatedAtAction("Details", new { id = publication.Id });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Cancellation handling - let attributes handle logging
                throw;
            }
            catch (Exception ex)
            {
                // Critical errors that attributes can't handle
                logger.LogError(ex, "Unhandled error in CreatePublication - Title: {Title}", model?.Title);
                return StatusCode(500, new { Message = "An unexpected error occurred while creating the publication." });
            }
        }

        // ============================================================================
        // IMPORTANT OPERATIONS - Attribute-Based Logging
        // ============================================================================

        [HttpGet("")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetAllPublications")]
        public async Task<ActionResult<PartPublicationsViewModel>> GetAllPublications(int? id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!id.HasValue)
                {
                    id = WebConstants.DefaultPage;
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);
                var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, 3, cancellationToken);

                if (publications == null || !publications.Any())
                {
                    return new PartPublicationsViewModel()
                    {
                        Id = id.Value,
                        Count = 0,
                        Publications = Enumerable.Empty<PublicationShortViewModel>()
                    };
                }

                return new PartPublicationsViewModel()
                {
                    Id = id.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetAllPublications API - Page: {Page}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving publications." });
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Attribute-Based Logging
        // ============================================================================

        [HttpGet("{id}", Name = "Details")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublication")]
        public async Task<IActionResult> GetPublication(int id, CancellationToken cancellationToken = default) 
        {
            try
            {
                var publicationModel = await this.BaseService.GetPublicationAsync(id, cancellationToken);
                
                if (publicationModel == null)
                {
                    return NotFound(new { Message = "The publication does not exist." });
                }

                return Ok(publicationModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
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
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(cancellationToken);
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            return requiredPagesForThisPublications;
        }
    }
}
