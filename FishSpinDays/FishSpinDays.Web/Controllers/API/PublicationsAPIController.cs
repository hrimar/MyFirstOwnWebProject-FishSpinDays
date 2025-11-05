namespace FishSpinDays.Web.Controllers.API
{
    using FishSpinDays.Common.API.Models.Publications;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Identity.BindingModels;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Web.Helpers.Filters;
    using FishSpinDays.Web.Mapping;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// API Controller for managing fishing publications
    /// Provides endpoints for creating, retrieving, and searching publications
    /// </summary>
    [Route("api/publications")] // for url: http://localhost:44331/api/publications
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiSecurityValidation]
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

        /// <summary>
        /// Creates a new publication
        /// </summary>
        /// <param name="model">Publication data including title, description, and section</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created publication with ID and success message</returns>
        /// <response code="201">Publication created successfully</response>
        /// <response code="400">Invalid model data</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User or section not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("")]
        [Authorize] // To use this for API makes AuthController
        [CriticalOperation(OperationName = "CreatePublication", SlowThresholdMs = 1500)]
        [ApiSecurityLogging]
        [ApiMetrics]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of publications with total page count</returns>
        /// <response code="200">Returns the paginated publications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetAllPublications")]
        [ProducesResponseType(typeof(PartPublicationsResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PartPublicationsResponseModel>> GetAllPublications(int? page, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!page.HasValue)
                {
                    page = WebConstants.DefaultPage;
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);
                var publications = await this.BaseService.GetAllPublicationsAsync(page.Value, WebConstants.DefaultResultCount, cancellationToken);

                // Map each publication individually and filter out nulls (defensive programming)
                var mappedPublications = publications?
                    .Select(p => PublicationApiMapper.ToShortResponseModel(p))
                    .Where(p => p != null) // Filter out any null results from mapper
                    ?? Enumerable.Empty<PublicationShortResponseModel>();

                var response = new PartPublicationsResponseModel
                {
                    Id = page.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = mappedPublications
                };

                return response;
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

        /// <summary>
        /// Get sea fishing publications with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of sea fishing publications</returns>
        /// <response code="200">Returns the paginated sea fishing publications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sea")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetSeaPublications")]
        [ProducesResponseType(typeof(PartPublicationsViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get freshwater fishing publications with pagination
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of freshwater fishing publications</returns>
        /// <response code="200">Returns the paginated freshwater fishing publications</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("freshwater")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetFreshwaterPublications")]
        [ProducesResponseType(typeof(PartPublicationsViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get the most rated (most liked) publication
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The publication with the highest number of likes</returns>
        /// <response code="200">Returns the most rated publication</response>
        /// <response code="404">No publications found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("most-rated")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetMostRatedPublication")]
        [ProducesResponseType(typeof(PublicationResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PublicationResponseModel>> GetMostRatedPublication(CancellationToken cancellationToken = default)
        {
            try
            {
                var publicationModel = await this.BaseService.MostReadedAsync(cancellationToken);

                if (publicationModel == null)
                {
                    return NotFound(new { Message = "No publications found." });
                }

                var response = PublicationApiMapper.ToResponseModel(publicationModel);

                // Defensive null check after mapping
                if (response == null)
                {
                    logger.LogError("Mapper returned null for most rated publication. Publication ID: {Id}", publicationModel.Id);
                    return StatusCode(500, new { Message = "An error occurred while processing the publication." });
                }

                return Ok(response);
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

        /// <summary>
        /// Get all publications in the 'Rods and Reels' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about rods and reels</returns>
        /// <response code="200">Returns publications from the rods section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/rods")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetRodsPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Lures' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about fishing lures</returns>
        /// <response code="200">Returns publications from the lures section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/lures")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetLuresPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Handmade Lures' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about handmade fishing lures</returns>
        /// <response code="200">Returns publications from the handmade lures section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/handmade")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetHandmadePublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Eco Fishing' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about ecological fishing practices</returns>
        /// <response code="200">Returns publications from the eco fishing section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/eco")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetEcoPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Fishing Schools' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about fishing schools and education</returns>
        /// <response code="200">Returns publications from the fishing schools section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/school")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetSchoolPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Anti-Poaching' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about anti-poaching and conservation</returns>
        /// <response code="200">Returns publications from the anti-poaching section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/anti")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetAntiPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications in the 'Fish Breeding' section
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications about fish breeding and aquaculture</returns>
        /// <response code="200">Returns publications from the fish breeding section</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("sections/breeding")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetBreedingPublications")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications from a specific year
        /// </summary>
        /// <param name="year">Year to filter by (e.g., 2024)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications from the specified year</returns>
        /// <response code="200">Returns publications from the specified year</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("year/{year}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationsByYear")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get all publications from a specific month
        /// </summary>
        /// <param name="month">Month number (1-12)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications from the specified month</returns>
        /// <response code="200">Returns publications from the specified month</response>
        /// <response code="400">Invalid month number (must be 1-12)</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("month/{month}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationsByMonth")]
        [ProducesResponseType(typeof(IEnumerable<PublicationShortViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Get a specific publication by ID
        /// </summary>
        /// <param name="id">Publication ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Publication details including title, description, author, and comments</returns>
        /// <response code="200">Returns the requested publication</response>
        /// <response code="404">Publication not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}", Name = "GetPublication")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublication")]
        [ProducesResponseType(typeof(PublicationViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Like a publication (increment likes counter)
        /// </summary>
        /// <param name="id">Publication ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success message with updated likes count</returns>
        /// <response code="200">Publication liked successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Publication not found</response>
        /// <response code="500">Failed to like publication</response>
        [HttpPost("{id}/like")]
        [Authorize]
        [SimpleOperation(OperationName = "LikePublication")]
        [ProducesResponseType(typeof(PublicationLikeResultModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                    // Reload publication to get updated likes count
                    var updatedPublication = await this.identityService.GetPublicationByIdAsync(id, cancellationToken);
                    return Ok(new PublicationLikeResultModel { Message = "Publication liked successfully.", NewLikesCount = updatedPublication.Likes });
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

        /// <summary>
        /// Get publication statistics (total counts by category)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Statistics including total, sea, freshwater, and other publications count</returns>
        /// <response code="200">Returns publication statistics</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("stats")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPublicationStats")]
        [ProducesResponseType(typeof(PublicationStatsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPublicationStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var totalPublications = await this.BaseService.TotalPublicationsCountAsync(cancellationToken);
                var seaPublications = await this.BaseService.TotalPublicationsCountAsync(WebConstants.SeaSection, cancellationToken);
                var freshwaterPublications = await this.BaseService.TotalPublicationsCountAsync(WebConstants.FreshwaterSection, cancellationToken);

                var stats = new PublicationStatsResponse
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

        /// <summary>
        /// Search publications by keyword in description
        /// </summary>
        /// <param name="searchTerm">Search keyword (minimum 3 characters)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of publications matching the search criteria</returns>
        /// <response code="200">Returns matching publications</response>
        /// <response code="400">Invalid search term (empty or too short)</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("search")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "SearchPublications")]
        [ProducesResponseType(typeof(List<SearchPublicationResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SearchPublicationResponseModel>>> SearchPublications([FromQuery] string searchTerm, CancellationToken cancellationToken = default)
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
 
                // Map results and filter out nulls (defensive programming)
                var response = results?
                    .Select(r => PublicationApiMapper.ToSearchResponseModel(r))
                    .Where(r => r != null) // Filter out any null results from mapper
                    .ToList() 
                  ?? new List<SearchPublicationResponseModel>();

                return Ok(response);
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