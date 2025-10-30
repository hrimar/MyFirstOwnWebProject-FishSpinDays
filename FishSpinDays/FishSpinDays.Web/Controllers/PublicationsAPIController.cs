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
    using System.Collections.Generic;

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
        // CRITICAL OPERATIONS - Full Logging with Performance Monitoring
        // ============================================================================

        [HttpPost("")]
        [Authorize] // To use this for API makes AuthController
        [CriticalOperation(OperationName = "CreatePublication", SlowThresholdMs = 1500)]
        [ApiSecurityLogging]
        [ApiMetrics]
        public async Task<IActionResult> CreatePublication([FromBody] PublicationBindingModel model, CancellationToken cancellationToken = default)
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

                return CreatedAtAction("GetPublication", new { id = publication.Id }, new { Id = publication.Id, Message = "Publication created successfully." });
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
        // IMPORTANT OPERATIONS - Moderate Logging  
        // ============================================================================

        [HttpGet("")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetAllPublications")]
        public async Task<ActionResult<PartPublicationsViewModel>> GetAllPublications(int? page, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!page.HasValue)
                {
                    page = WebConstants.DefaultPage;
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);
                var publications = await this.BaseService.GetAllPublicationsAsync(page.Value, WebConstants.DefaultResultCount, cancellationToken);

                if (publications == null || !publications.Any())
                {
                    return new PartPublicationsViewModel()
                    {
                        Id = page.Value,
                        Count = 0,
                        Publications = Enumerable.Empty<PublicationShortViewModel>()
                    };
                }

                return new PartPublicationsViewModel()
                {
                    Id = page.Value,
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
                logger.LogError(ex, "Error in GetAllPublications API - Page: {Page}", page);
                return StatusCode(500, new { Message = "An error occurred while retrieving publications." });
            }
        }

        [HttpGet("sea")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetSeaPublications")]
        public async Task<ActionResult<PartPublicationsViewModel>> GetSeaPublications(int? page, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!page.HasValue)
                {
                    page = WebConstants.DefaultPage;
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.SeaSection, cancellationToken);
                var publications = await this.BaseService.GetAllSeaPublicationsAsync(page.Value, WebConstants.DefaultResultCount, cancellationToken);

                if (publications == null || !publications.Any())
                {
                    return new PartPublicationsViewModel()
                    {
                        Id = page.Value,
                        Count = 0,
                        Publications = Enumerable.Empty<PublicationShortViewModel>()
                    };
                }

                return new PartPublicationsViewModel()
                {
                    Id = page.Value,
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
                logger.LogError(ex, "Error in GetSeaPublications API - Page: {Page}", page);
                return StatusCode(500, new { Message = "An error occurred while retrieving sea publications." });
            }
        }

        [HttpGet("freshwater")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetFreshwaterPublications")]
        public async Task<ActionResult<PartPublicationsViewModel>> GetFreshwaterPublications(int? page, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!page.HasValue)
                {
                    page = WebConstants.DefaultPage;
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.FreshwaterSection, cancellationToken);
                var publications = await this.BaseService.GetAllFreshwaterPublicationsAsync(page.Value, WebConstants.DefaultResultCount, cancellationToken);

                return new PartPublicationsViewModel()
                {
                    Id = page.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications ?? Enumerable.Empty<PublicationShortViewModel>()
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetFreshwaterPublications API - Page: {Page}", page);
                return StatusCode(500, new { Message = "An error occurred while retrieving freshwater publications." });
            }
        }

        [HttpGet("most-rated")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetMostRatedPublication")]
        public async Task<ActionResult<PublicationViewModel>> GetMostRatedPublication(CancellationToken cancellationToken = default)
        {
            try
            {
                var publicationModel = await this.BaseService.MostReadedAsync(cancellationToken);

                if (publicationModel == null)
                {
                    return NotFound(new { Message = "No publications found." });
                }

                return Ok(publicationModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting most rated publication via API");
                return StatusCode(500, new { Message = "An error occurred while retrieving the most rated publication." });
            }
        }

        // ============================================================================
        // SECTION-BASED OPERATIONS
        // ============================================================================

        [HttpGet("sections/rods")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetRodsPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetRodsPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Rods, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetRodsPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving rods publications." });
            }
        }

        [HttpGet("sections/lures")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetLuresPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetLuresPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Lures, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetLuresPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving lures publications." });
            }
        }

        [HttpGet("sections/handmade")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetHandmadePublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetHandmadePublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.HandLures, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetHandmadePublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving handmade publications." });
            }
        }

        [HttpGet("sections/eco")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetEcoPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetEcoPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Eco, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetEcoPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving eco publications." });
            }
        }

        [HttpGet("sections/school")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetSchoolPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetSchoolPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.School, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetSchoolPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving school publications." });
            }
        }

        [HttpGet("sections/anti")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetAntiPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetAntiPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Anti, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetAntiPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving anti-poaching publications." });
            }
        }

        [HttpGet("sections/breeding")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetBreedingPublications")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetBreedingPublications(CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Breeding, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetBreedingPublications API");
                return StatusCode(500, new { Message = "An error occurred while retrieving breeding publications." });
            }
        }

        // ============================================================================
        // TIME-BASED OPERATIONS
        // ============================================================================

        [HttpGet("year/{year}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationsByYear")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetPublicationsByYear(int year, CancellationToken cancellationToken = default)
        {
            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisYearAsync(year, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetPublicationsByYear API - Year: {Year}", year);
                return StatusCode(500, new { Message = "An error occurred while retrieving publications for the specified year." });
            }
        }

        [HttpGet("month/{month}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationsByMonth")]
        public async Task<ActionResult<IEnumerable<PublicationShortViewModel>>> GetPublicationsByMonth(int month, CancellationToken cancellationToken = default)
        {
            try
            {
                if (month < 1 || month > 12)
                {
                    return BadRequest(new { Message = "Month must be between 1 and 12." });
                }

                var publications = await this.BaseService.GetAllPublicationsInThisMonthAsync(month, cancellationToken);
                return Ok(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetPublicationsByMonth API - Month: {Month}", month);
                return StatusCode(500, new { Message = "An error occurred while retrieving publications for the specified month." });
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Minimal Logging
        // ============================================================================

        [HttpGet("{id}", Name = "GetPublication")]
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

        [HttpPost("{id}/like")]
        [Authorize]
        [SimpleOperation(OperationName = "LikePublication")]
        public async Task<IActionResult> LikePublication(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var publication = await this.identityService.GetPublicationByIdAsync(id, cancellationToken);

                if (publication == null)
                {
                    return NotFound(new { Message = "Publication not found." });
                }

                var success = await this.identityService.IsLikedPublicationAsync(publication, cancellationToken);

                if (success)
                {
                    return Ok(new { Message = "Publication liked successfully.", NewLikesCount = publication.Likes });
                }

                return StatusCode(500, new { Message = "Failed to like publication." });
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error liking publication via API - ID: {PublicationId}", id);
                return StatusCode(500, new { Message = "An error occurred while liking the publication." });
            }
        }

        // ============================================================================
        // UTILITY ENDPOINTS
        // ============================================================================

        [HttpGet("stats")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationStats")]
        public async Task<IActionResult> GetPublicationStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var totalPublications = await this.BaseService.TotalPublicationsCountAsync(cancellationToken);
                var seaPublications = await this.BaseService.TotalPublicationsCountAsync(WebConstants.SeaSection, cancellationToken);
                var freshwaterPublications = await this.BaseService.TotalPublicationsCountAsync(WebConstants.FreshwaterSection, cancellationToken);

                var stats = new
                {
                    TotalPublications = totalPublications,
                    SeaPublications = seaPublications,
                    FreshwaterPublications = freshwaterPublications,
                    OtherPublications = totalPublications - seaPublications - freshwaterPublications
                };

                return Ok(stats);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting publication stats via API");
                return StatusCode(500, new { Message = "An error occurred while retrieving publication statistics." });
            }
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "SearchPublications")]
        public async Task<ActionResult<List<SearchPublicationViewModel>>> SearchPublications([FromQuery] string searchTerm, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { Message = "Search term cannot be empty." });
                }

                if (searchTerm.Length < 3)
                {
                    return BadRequest(new { Message = "Search term must be at least 3 characters long." });
                }

                var results = await this.identityService.FoundPublicationsAsync(searchTerm, cancellationToken);
                return Ok(results ?? new List<SearchPublicationViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching publications via API - SearchTerm: {SearchTerm}", searchTerm);
                return StatusCode(500, new { Message = "An error occurred while searching publications." });
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

        private async Task<int> ArrangePagesCountAsync(string type, CancellationToken cancellationToken = default)
        {
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(type, cancellationToken);
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
