namespace FishSpinDays.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Web.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Constants;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using System;

    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(IBasePublicationsService baseService, ILogger<HomeController> logger)
            : base(baseService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================================
        // CRITICAL OPERATIONS - Full Logging with Performance Monitoring
        // ============================================================================

        [HttpGet]
        public async Task<IActionResult> Index(int? id, CancellationToken cancellationToken = default)
        {
            var userAgent = GetUserAgentSafe();
            using var scope = logger.BeginScope("Index - Page: {Page}, UserAgent: {UserAgent}", id?.ToString() ?? "null", userAgent);
            using var activity = new Activity("HomeIndex");
            activity.Start();
            
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting Index page request with cancellation support");

            try
            {
                if (!id.HasValue)
                {
                    id = WebConstants.DefaultPage;
                    logger.LogDebug("No page specified, using default page: {DefaultPage}", id);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Index request was cancelled before processing - Page: {Page}", id);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Getting page count and publications for page {Page}", id);

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);
                var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken);

                // Handle empty results gracefully instead of NotFound
                if (publications == null || !publications.Any())
                {
                    logger.LogWarning("No publications found for page {Page} after {ElapsedMs}ms - showing empty page", id, stopwatch.ElapsedMilliseconds);
                    publications = Enumerable.Empty<PublicationShortViewModel>();
                    requiredPagesForThisPublications = 0;
                }

                var viewModel = new PartPublicationsViewModel()
                {
                    Id = id.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications
                };

                stopwatch.Stop();
                logger.LogInformation("Index page completed successfully - Page: {Page}, PublicationCount: {PublicationCount}, TotalPages: {TotalPages}, Duration: {ElapsedMs}ms", 
                    id, publications.Count(), requiredPagesForThisPublications, stopwatch.ElapsedMilliseconds);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Slow Index page operation - Page: {Page} took {ElapsedMs}ms", id, stopwatch.ElapsedMilliseconds);
                }

                return View(viewModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Index request was cancelled after {ElapsedMs}ms - Page: {Page}", 
                    stopwatch.ElapsedMilliseconds, id);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }

        // ============================================================================
        // IMPORTANT OPERATIONS - Moderate Logging
        // ============================================================================

        public async Task<IActionResult> Sea(int? id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("Sea - Page: {Page}", id?.ToString() ?? "null");
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting Sea publications request");

            try
            {
                if (!id.HasValue)
                {
                    id = WebConstants.DefaultPage;
                    logger.LogDebug("Using default page for sea publications: {DefaultPage}", id);
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.SeaSection, cancellationToken);
                var publications = await this.BaseService.GetAllSeaPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken);
                
                // Handle empty results gracefully instead of NotFound
                if (publications == null || !publications.Any())
                {
                    logger.LogWarning("No sea publications found for page {Page} after {ElapsedMs}ms - showing empty page", id, stopwatch.ElapsedMilliseconds);
                    publications = Enumerable.Empty<PublicationShortViewModel>();
                    requiredPagesForThisPublications = 0;
                }

                var viewModel = new PartPublicationsViewModel()
                {
                    Id = id.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications
                };

                stopwatch.Stop();
                logger.LogInformation("Sea publications page completed - Page: {Page}, Count: {PublicationCount}, Duration: {ElapsedMs}ms", 
                    id, publications.Count(), stopwatch.ElapsedMilliseconds);

                return View(viewModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Sea publications request was cancelled after {ElapsedMs}ms - Page: {Page}", 
                    stopwatch.ElapsedMilliseconds, id);
                throw;
            }
        }

        public async Task<IActionResult> Freshwater(int? id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("Freshwater - Page: {Page}", id?.ToString() ?? "null");
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting Freshwater publications request");

            try
            {
                if (!id.HasValue)
                {
                    id = WebConstants.DefaultPage;
                    logger.LogDebug("Using default page for freshwater publications: {DefaultPage}", id);
                }

                int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.FreshwaterSection, cancellationToken);
                var publications = (await this.BaseService.GetAllFreshwaterPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken))?.ToList() ?? Enumerable.Empty<PublicationShortViewModel>().ToList();

                var viewModel = new PartPublicationsViewModel()
                {
                    Id = id.Value,
                    Count = requiredPagesForThisPublications,
                    Publications = publications
                };

                stopwatch.Stop();
                logger.LogInformation("Freshwater publications page completed - Page: {Page}, Count: {PublicationCount}, Duration: {ElapsedMs}ms", 
                    id, publications.Count, stopwatch.ElapsedMilliseconds);

                return View(viewModel);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Freshwater publications request was cancelled after {ElapsedMs}ms - Page: {Page}", stopwatch.ElapsedMilliseconds, id);
                throw;
            }
        }

        public async Task<IActionResult> Rods(CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("Rods");
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting Rods section request");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Rods, cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Rods section completed - Count: {PublicationCount}, Duration: {ElapsedMs}ms", 
                    publications?.Count() ?? 0, stopwatch.ElapsedMilliseconds);

                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Rods section request was cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        public async Task<IActionResult> Lures(CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("Lures");
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting Lures section request");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Lures, cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Lures section completed - Count: {PublicationCount}, Duration: {ElapsedMs}ms", 
                    publications?.Count() ?? 0, stopwatch.ElapsedMilliseconds);

                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Lures section request was cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Minimal Logging
        // ============================================================================

        public async Task<IActionResult> Handmade(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting handmade publications");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.HandLures, cancellationToken);
                logger.LogInformation("Handmade section completed - Count: {PublicationCount}", publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Handmade section request was cancelled");
                throw;
            }
        }

        public async Task<IActionResult> Eco(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting eco publications");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Eco, cancellationToken);
                logger.LogInformation("Eco section completed - Count: {PublicationCount}", publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Eco section request was cancelled");
                throw;
            }
        }

        public async Task<IActionResult> School(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting school publications");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.School, cancellationToken);
                logger.LogInformation("School section completed - Count: {PublicationCount}", publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("School section request was cancelled");
                throw;
            }
        }

        public async Task<IActionResult> Anti(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting anti publications");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Anti, cancellationToken);
                logger.LogInformation("Anti section completed - Count: {PublicationCount}", publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Anti section request was cancelled");
                throw;
            }
        }

        public async Task<IActionResult> Breeding(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting breeding publications");

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Breeding, cancellationToken);
                logger.LogInformation("Breeding section completed - Count: {PublicationCount}", publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Breeding section request was cancelled");
                throw;
            }
        }

        public async Task<IActionResult> Year(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting publications for year {Year}", WebConstants.Year2018);

            try
            {
                var publications = await this.BaseService.GetAllPublicationsInThisYearAsync(WebConstants.Year2018, cancellationToken);
                logger.LogInformation("Year {Year} publications completed - Count: {PublicationCount}", 
                    WebConstants.Year2018, publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Year {Year} request was cancelled", WebConstants.Year2018);
                throw;
            }
        }

        public async Task<IActionResult> Month(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting publications for month {Month}", id);

            try
            {
                this.ViewData["Month"] = id;
                var publications = await this.BaseService.GetAllPublicationsInThisMonthAsync(id, cancellationToken);
                
                logger.LogInformation("Month {Month} publications completed - Count: {PublicationCount}", 
                    id, publications?.Count() ?? 0);
                
                return View(publications ?? Enumerable.Empty<PublicationShortViewModel>());
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Month {Month} request was cancelled", id);
                throw;
            }
        }

        // ============================================================================
        // STATIC PAGES - Basic Logging
        // ============================================================================

        public IActionResult Contact()
        {
            logger.LogDebug("Contact page accessed");
            return View();
        }

        public IActionResult Privacy()
        {
            logger.LogDebug("Privacy page accessed");
            return View();
        }

        public IActionResult Info()
        {
            logger.LogDebug("Info page accessed");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/Error")]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            logger.LogWarning("Error page displayed - RequestId: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Home/Error/{statusCode:int}")]
        public IActionResult Error(int statusCode)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            logger.LogWarning("Error page displayed for status code {StatusCode} - RequestId: {RequestId}",
                statusCode, requestId);

            var errorViewModel = new ErrorViewModel
            {
                RequestId = requestId,
                StatusCode = statusCode,
                StatusMessage = GetStatusMessage(statusCode)
            };

            return View("Error", errorViewModel);
        }

        private string GetStatusMessage(int statusCode)
        {
            return statusCode switch
            {
                404 => "The page you are looking for does not exist.",
                403 => "You do not have permission to access this page.",
                500 => "An error occurred while processing your request.",
                _ => "An unexpected error occurred."
            };
        }

        // ============================================================================
        // PRIVATE HELPER METHODS
        // ============================================================================

        private async Task<int> ArrangePagesCountAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Calculating total page count");

            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(cancellationToken);
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            logger.LogDebug("Page count calculation - Total publications: {TotalPublications}, Total pages: {TotalPages}", 
                totalPublicationsCount, requiredPagesForThisPublications);

            return requiredPagesForThisPublications;
        }

        private async Task<int> ArrangePagesCountAsync(string type, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Calculating page count for type: {Type}", type);
            
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(type, cancellationToken);
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            logger.LogDebug("Page count calculation for {Type} - Total publications: {TotalPublications}, Total pages: {TotalPages}", 
                type, totalPublicationsCount, requiredPagesForThisPublications);

            return requiredPagesForThisPublications;
        }

        private string GetUserAgentSafe()
        {
            return Request?.Headers?.UserAgent.FirstOrDefault() ?? "Unknown";
        }
    }
}
