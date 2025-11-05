namespace FishSpinDays.Web.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using FishSpinDays.Web.Helpers.Filters;
    using System.Linq;
    using AutoMapper;
    using FishSpinDays.Common.API.Models.Stats;
    using FishSpinDays.Common.API.Models.Publications;

    /// <summary>
    /// API Controller for application statistics and analytics
    /// </summary>
    [Route("api/stats")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [ApiSecurityValidation]
    public class StatsAPIController : ControllerBase
    {
        private readonly IBasePublicationsService basePublicationsService;
        private readonly IIdentityService identityService;
        private readonly ILogger<StatsAPIController> logger;
        private readonly IMapper mapper;

        public StatsAPIController(
            IBasePublicationsService basePublicationsService,
            IIdentityService identityService,
            ILogger<StatsAPIController> logger,
            IMapper mapper)
        {
            this.basePublicationsService = basePublicationsService;
            this.identityService = identityService;
            this.logger = logger;
            this.mapper = mapper;
        }

        /// <summary>
        /// Get overall application statistics
        /// </summary>
        [HttpGet("overview")]
        [AllowAnonymous]
        [ImportantOperation(OperationName = "GetOverviewStats")]
        public async Task<IActionResult> GetOverviewStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var totalPublications = await basePublicationsService.TotalPublicationsCountAsync(cancellationToken);
                var seaPublications = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.SeaSection, cancellationToken);
                var freshwaterPublications = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.FreshwaterSection, cancellationToken);
                var currentYearPublications = await GetCurrentYearPublicationsCountAsync(cancellationToken);
                var currentMonthPublications = await GetCurrentMonthPublicationsCountAsync(cancellationToken);

                var result = new OverviewStatsResponse
                {
                    TotalPublications = totalPublications,
                    SeaPublications = seaPublications,
                    FreshwaterPublications = freshwaterPublications,
                    TacklePublications = await GetTacklePublicationsCountAsync(cancellationToken),
                    ActionsPublications = await GetActionsPublicationsCountAsync(cancellationToken),
                    CurrentYearPublications = currentYearPublications,
                    CurrentMonthPublications = currentMonthPublications,
                    OtherPublications = totalPublications - seaPublications - freshwaterPublications,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting overview statistics");
                return StatusCode(500, new { Message = "An error occurred while retrieving overview statistics." });
            }
        }

        /// <summary>
        /// Get detailed section statistics
        /// </summary>
        [HttpGet("sections")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetSectionStats")]
        public async Task<IActionResult> GetSectionStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var sectionStats = new List<SectionCountItem>
                {
                    new SectionCountItem { Name = WebConstants.SeaSection, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.SeaSection, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.FreshwaterSection, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.FreshwaterSection, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.Rods, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Rods, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.Lures, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Lures, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.HandLures, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.HandLures, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.Eco, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Eco, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.School, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.School, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.Anti, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Anti, cancellationToken) },
                    new SectionCountItem { Name = WebConstants.Breeding, Count = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Breeding, cancellationToken) }
                }.OrderByDescending(s => s.Count);

                return Ok(sectionStats);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting section statistics");
                return StatusCode(500, new { Message = "An error occurred while retrieving section statistics." });
            }
        }

        /// <summary>
        /// Get time-based statistics (monthly/yearly trends)
        /// </summary>
        [HttpGet("trends")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetTrendStats")]
        public async Task<IActionResult> GetTrendStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                var currentMonth = DateTime.UtcNow.Month;

                // Get monthly stats for current year
                var monthlyStats = new List<TrendMonthlyItem>();
                for (int month = 1; month <= 12; month++)
                {
                    var publications = await basePublicationsService.GetAllPublicationsInThisMonthAsync(month, cancellationToken);
                    monthlyStats.Add(new TrendMonthlyItem
                    {
                        Month = month,
                        MonthName = new DateTime(currentYear, month, 1).ToString("MMMM"),
                        Count = publications?.Count() ?? 0,
                        IsCurrent = month == currentMonth
                    });
                }

                // Get yearly stats
                var yearlyStats = new TrendYearlyItem
                {
                    CurrentYear = currentYear,
                    Count = await GetCurrentYearPublicationsCountAsync(cancellationToken),
                    PreviousYear = currentYear - 1
                };

                var trends = new TrendStatsResponse
                {
                    Monthly = monthlyStats,
                    Yearly = yearlyStats,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(trends);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting trend statistics");
                return StatusCode(500, new { Message = "An error occurred while retrieving trend statistics." });
            }
        }

        /// <summary>
        /// Get most popular content
        /// </summary>
        [HttpGet("popular")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetPopularStats")]
        public async Task<IActionResult> GetPopularStats(CancellationToken cancellationToken = default)
        {
            try
            {
                var mostRatedPublication = await basePublicationsService.MostReadedAsync(cancellationToken);
                var popularStats = new PopularStatsResponse
                {
                    MostRatedPublication = mostRatedPublication != null ? new PublicationShortResponseModel
                    {
                        Id = mostRatedPublication.Id,
                        Title = mostRatedPublication.Title,
                        Likes = mostRatedPublication.Likes,
                        Author = mostRatedPublication.Author,
                        Section = mostRatedPublication.Section,
                        CreationDate = mostRatedPublication.CreationDate,
                        CommentsCount =0
                    } : null,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(popularStats);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting popular statistics");
                return StatusCode(500, new { Message = "An error occurred while retrieving popular statistics." });
            }
        }

        private async Task<int> GetCurrentYearPublicationsCountAsync(CancellationToken cancellationToken)
        {
            var currentYearPublications = await basePublicationsService.GetAllPublicationsInThisYearAsync(DateTime.UtcNow.Year, cancellationToken);
            return currentYearPublications?.Count() ?? 0;
        }

        private async Task<int> GetCurrentMonthPublicationsCountAsync(CancellationToken cancellationToken)
        {
            var currentMonthPublications = await basePublicationsService.GetAllPublicationsInThisMonthAsync(DateTime.UtcNow.Month, cancellationToken);
            return currentMonthPublications?.Count() ?? 0;
        }

        private async Task<int> GetTacklePublicationsCountAsync(CancellationToken cancellationToken)
        {
            var rodsCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Rods, cancellationToken);
            var luresCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Lures, cancellationToken);
            var handmadeCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.HandLures, cancellationToken);
            return rodsCount + luresCount + handmadeCount;
        }

        private async Task<int> GetActionsPublicationsCountAsync(CancellationToken cancellationToken)
        {
            var ecoCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Eco, cancellationToken);
            var schoolCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.School, cancellationToken);
            var antiCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Anti, cancellationToken);
            var breedingCount = await basePublicationsService.TotalPublicationsCountAsync(WebConstants.Breeding, cancellationToken);
            return ecoCount + schoolCount + antiCount + breedingCount;
        }
    }
}