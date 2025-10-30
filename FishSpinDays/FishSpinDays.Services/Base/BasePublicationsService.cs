namespace FishSpinDays.Services.Base
{
    using AutoMapper;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FishSpinDays.Common.Extensions;
    using FishSpinDays.Common.Base.ViewModels;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;

    public class BasePublicationsService : BaseService, IBasePublicationsService
    {
        private readonly ILogger<BasePublicationsService> logger;

        public BasePublicationsService(FishSpinDaysDbContext dbContex, IMapper mapper, ILogger<BasePublicationsService> logger)
            : base(dbContex, mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================================
        // CRITICAL OPERATIONS - Full Logging with Performance Monitoring
        // ============================================================================

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetAllPublications - Page: {Page}, Count: {Count}", page, count);
            using var activity = new Activity("GetAllPublications");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async GetAllPublications operation with cancellation support");

            try
            {
                if (page <= 0)
                {
                    logger.LogWarning("Invalid page number provided: {Page}", page);
                    return Enumerable.Empty<PublicationShortViewModel>();
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Operation was cancelled before database query - Page: {Page}", page);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Executing database query with eager loading for authors");
                var publications = await this.DbContext.Publications
                    .Include(p => p.Author)
                    .OrderByDescending(p => p.CreationDate)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                logger.LogDebug("Database query completed - Retrieved {PublicationCount} publications in {ElapsedMs}ms",
                    publications.Count, stopwatch.ElapsedMilliseconds);

                var model = GetAllTargetPublicationsWithLoadedAuthors(publications);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed GetAllPublications - Page: {Page}, Count: {ResultCount}, Duration: {ElapsedMs}ms",
                    page, model.Count, stopwatch.ElapsedMilliseconds);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Slow operation detected - GetAllPublications took {ElapsedMs}ms for page {Page}",
                        stopwatch.ElapsedMilliseconds, page);
                }

                return model;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetAllPublications was cancelled after {ElapsedMs}ms - Page: {Page}",
                    stopwatch.ElapsedMilliseconds, page);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error occurred in GetAllPublications after {ElapsedMs}ms - Page: {Page}, Count: {Count}",
                    stopwatch.ElapsedMilliseconds, page, count);

                return Enumerable.Empty<PublicationShortViewModel>();
            }
            finally
            {
                activity.Stop();
            }
        }

        public async Task<PublicationViewModel> GetPublicationAsync(int id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetPublication - ID: {PublicationId}", id);
            using var activity = new Activity("GetPublication");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async GetPublication operation with cancellation support");

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Operation was cancelled before database query - PublicationId: {PublicationId}", id);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Executing database query with full eager loading");
                var publication = await this.DbContext.Publications
                    .Include(s => s.Comments).ThenInclude(c => c.Author)
                    .Include(s => s.Author)
                    .Include(s => s.Section)
                    .FirstOrDefaultAsync(s => s.Id == id, cancellationToken).ConfigureAwait(false);

                stopwatch.Stop();

                if (publication == null)
                {
                    logger.LogWarning("Publication not found after {ElapsedMs}ms - ID: {PublicationId}",
                        stopwatch.ElapsedMilliseconds, id);
                }
                else
                {
                    logger.LogDebug("Publication found after {ElapsedMs}ms - ID: {PublicationId}, Title: {Title}, Comments: {CommentCount}",
                        stopwatch.ElapsedMilliseconds, id, publication.Title, publication.Comments?.Count ?? 0);
                }

                var model = this.Mapper.Map<PublicationViewModel>(publication);

                logger.LogInformation("Successfully completed GetPublication - ID: {PublicationId}, Found: {Found}, Duration: {ElapsedMs}ms",
                    id, model != null, stopwatch.ElapsedMilliseconds);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    logger.LogWarning("Slow GetPublication operation - ID: {PublicationId} took {ElapsedMs}ms",
                        id, stopwatch.ElapsedMilliseconds);
                }

                return model;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetPublication was cancelled after {ElapsedMs}ms - ID: {PublicationId}",
                    stopwatch.ElapsedMilliseconds, id);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error occurred in GetPublication after {ElapsedMs}ms - ID: {PublicationId}",
                    stopwatch.ElapsedMilliseconds, id);

                return null;
            }
            finally
            {
                activity.Stop();
            }
        }

        public async Task<int> TotalPublicationsCountAsync(CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("TotalPublicationsCount");
            using var activity = new Activity("TotalPublicationsCount");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async TotalPublicationsCount operation");

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("TotalPublicationsCount operation was cancelled before database query");
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var count = await this.DbContext.Publications.CountAsync(cancellationToken).ConfigureAwait(false);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed TotalPublicationsCount - Count: {Count}, Duration: {ElapsedMs}ms",
                    count, stopwatch.ElapsedMilliseconds);

                return count;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("TotalPublicationsCount was cancelled after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in TotalPublicationsCount after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }

        public async Task<int> TotalPublicationsCountAsync(string type, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("TotalPublicationsCountByType - Type: {Type}", type);
            using var activity = new Activity("TotalPublicationsCountByType");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async TotalPublicationsCount by type operation");

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Operation was cancelled before database query - Type: {Type}", type);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var count = await this.DbContext.Publications
                    .Include(p => p.Section)
                    .Where(p => p.Section.Name == type)
                    .CountAsync(cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed TotalPublicationsCountByType - Type: {Type}, Count: {Count}, Duration: {ElapsedMs}ms",
                    type, count, stopwatch.ElapsedMilliseconds);

                return count;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("TotalPublicationsCountByType was cancelled after {ElapsedMs}ms - Type: {Type}",
                    stopwatch.ElapsedMilliseconds, type);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in TotalPublicationsCountByType after {ElapsedMs}ms - Type: {Type}",
                    stopwatch.ElapsedMilliseconds, type);
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

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllSeaPublicationsAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetAllSeaPublications - Page: {Page}, Count: {Count}", page, count);
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting GetAllSeaPublications operation");

            try
            {
                var publications = await GetTargetSectionAsync(WebConstants.SeaSection, page, count, cancellationToken);
                if (publications == null)
                {
                    logger.LogWarning("No sea publications found for page {Page} after {ElapsedMs}ms", page, stopwatch.ElapsedMilliseconds);
                    return Enumerable.Empty<PublicationShortViewModel>();
                }

                var model = GetAllTargetPublicationsWithLoadedAuthors(publications);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed GetAllSeaPublications - Page: {Page}, Count: {ResultCount}, Duration: {ElapsedMs}ms",
                    page, model.Count, stopwatch.ElapsedMilliseconds);

                // Only warn if slow
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Slow GetAllSeaPublications operation - Page: {Page} took {ElapsedMs}ms",
                        page, stopwatch.ElapsedMilliseconds);
                }

                return model;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetAllSeaPublications was cancelled after {ElapsedMs}ms - Page: {Page}",
                    stopwatch.ElapsedMilliseconds, page);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetAllSeaPublications after {ElapsedMs}ms - Page: {Page}",
                    stopwatch.ElapsedMilliseconds, page);
                return Enumerable.Empty<PublicationShortViewModel>();
            }
        }

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllFreshwaterPublicationsAsync(int page, int count, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetAllFreshwaterPublications - Page: {Page}, Count: {Count}", page, count);
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting GetAllFreshwaterPublications operation");

            try
            {
                var publications = await GetTargetSectionAsync(WebConstants.FreshwaterSection, page, count, cancellationToken);
                if (publications == null)
                {
                    logger.LogWarning("No freshwater publications found for page {Page} after {ElapsedMs}ms", page, stopwatch.ElapsedMilliseconds);
                    return Enumerable.Empty<PublicationShortViewModel>();
                }

                var model = GetAllTargetPublicationsWithLoadedAuthors(publications);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed GetAllFreshwaterPublications - Page: {Page}, Count: {ResultCount}, Duration: {ElapsedMs}ms",
                    page, model.Count, stopwatch.ElapsedMilliseconds);

                return model;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetAllFreshwaterPublications was cancelled after {ElapsedMs}ms - Page: {Page}", stopwatch.ElapsedMilliseconds, page);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetAllFreshwaterPublications after {ElapsedMs}ms - Page: {Page}",
                    stopwatch.ElapsedMilliseconds, page);
                return Enumerable.Empty<PublicationShortViewModel>();
            }
        }

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisSectionAsync(string sectionType, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetAllPublicationsInSection - Section: {SectionType}", sectionType);
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting GetAllPublicationsInSection operation");

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Operation was cancelled before database query - Section: {SectionType}", sectionType);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var publications = await this.DbContext.Publications
                    .Include(p => p.Author)
                    .Where(p => p.Section.Name == sectionType)
                    .Take(WebConstants.DefaultResultPerPage)
                    .ToListAsync(cancellationToken);

                var model = GetAllTargetPublicationsWithLoadedAuthors(publications);

                stopwatch.Stop();
                logger.LogInformation("Successfully completed GetAllPublicationsInSection - Section: {SectionType}, Count: {ResultCount}, Duration: {ElapsedMs}ms",
                    sectionType, model.Count, stopwatch.ElapsedMilliseconds);

                return model;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetAllPublicationsInSection was cancelled after {ElapsedMs}ms - Section: {SectionType}",
                    stopwatch.ElapsedMilliseconds, sectionType);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetAllPublicationsInSection after {ElapsedMs}ms - Section: {SectionType}",
                    stopwatch.ElapsedMilliseconds, sectionType);
                return Enumerable.Empty<PublicationShortViewModel>();
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Minimal Logging
        // ============================================================================

        public async Task<PublicationViewModel> MostReadedAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting most read publication");

            try
            {
                var publication = await this.DbContext.Publications
                    .OrderByDescending(p => p.Likes)
                    .Include(s => s.Comments)
                    .Include(s => s.Author)
                    .Include(s => s.Section)
                    .FirstAsync(cancellationToken);

                logger.LogInformation("Most read publication found - ID: {PublicationId}, Likes: {Likes}",
                    publication.Id, publication.Likes);

                return this.Mapper.Map<PublicationViewModel>(publication);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("MostReadedAsync was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting most read publication");
                return null;
            }
        }

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisYearAsync(int year, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting publications for year {Year}", year);

            try
            {
                var publications = await this.DbContext.Publications
                    .Include(p => p.Author)
                    .Where(p => p.CreationDate.Year == year)
                    .Take(WebConstants.DefaultResultPerPage)
                    .ToListAsync(cancellationToken);

                var result = GetAllTargetPublicationsWithLoadedAuthors(publications);
                logger.LogInformation("Found {Count} publications for year {Year}", result.Count, year);

                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetAllPublicationsInThisYearAsync cancelled - Year: {Year}", year);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting publications for year {Year}", year);
                return Enumerable.Empty<PublicationShortViewModel>();
            }
        }

        public async Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisMonthAsync(int month, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting publications for month {Month}", month);

            try
            {
                var publications = await this.DbContext.Publications
                    .Include(p => p.Author)
                    .Where(p => p.CreationDate.Month == month)
                    .Take(WebConstants.DefaultResultPerPage)
                    .ToListAsync(cancellationToken);

                var result = GetAllTargetPublicationsWithLoadedAuthors(publications);
                logger.LogInformation("Found {Count} publications for month {Month}", result.Count, month);

                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetAllPublicationsInThisMonthAsync cancelled - Month: {Month}", month);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting publications for month {Month}", month);
                return Enumerable.Empty<PublicationShortViewModel>();
            }
        }

        // ============================================================================
        // SYNCHRONOUS METHODS - Basic Logging (Legacy Support)
        // ============================================================================

        public IEnumerable<PublicationShortViewModel> GetAllPublications(int page, int count)
        {
            logger.LogDebug("Synchronous GetAllPublications - Page: {Page}, Count: {Count}", page, count);

            if (page <= 0)
            {
                logger.LogWarning("Invalid page number: {Page}", page);
                return null;
            }

            var publications = this.DbContext.Publications
                .OrderByDescending(p => p.CreationDate)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            var result = publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = GetAutorById(p.AuthorId),
                Description = p.Description.GetOnlyTextFromDescription(),
                CoverImage = GetMainImage(p.Description)
            }).ToList();

            logger.LogInformation("Processed {Count} publications for page {Page}", result.Count, page);
            return result;
        }

        public PublicationViewModel GetPublication(int id)
        {
            logger.LogDebug("Synchronous GetPublication - ID: {PublicationId}", id);

            var publication = this.DbContext.Publications
                .Include(s => s.Comments).ThenInclude(c => c.Author)
                .Include(s => s.Author)
                .Include(s => s.Section)
                .FirstOrDefault(s => s.Id == id);

            var model = this.Mapper.Map<PublicationViewModel>(publication);
            logger.LogInformation("Publication {PublicationId} {Status}", id, model != null ? "found" : "not found");

            return model;
        }

        public PublicationViewModel MostReaded()
        {
            logger.LogDebug("Synchronous MostReaded");

            var publication = this.DbContext.Publications
                .OrderByDescending(p => p.Likes)
                .Include(s => s.Comments)
                .Include(s => s.Author)
                .Include(s => s.Section)
                .First();

            logger.LogInformation("Most read publication: ID {PublicationId}, Likes: {Likes}", publication.Id, publication.Likes);

            return this.Mapper.Map<PublicationViewModel>(publication);
        }

        public IEnumerable<PublicationShortViewModel> GetAllSeaPublications(int page, int count)
        {
            logger.LogDebug("Synchronous GetAllSeaPublications - Page: {Page}", page);

            var publications = GetTargetSection(WebConstants.SeaSection, page, count);
            if (publications == null) return null;

            var result = GetAllTargetPublications(publications);
            logger.LogInformation("Found {Count} sea publications for page {Page}", result.Count, page);

            return result;
        }

        public IEnumerable<PublicationShortViewModel> GetAllFreshwaterPublications(int page, int count)
        {
            logger.LogDebug("Synchronous GetAllFreshwaterPublications - Page: {Page}", page);

            var publications = GetTargetSection(WebConstants.FreshwaterSection, page, count);
            if (publications == null) return null;

            var result = GetAllTargetPublications(publications);
            logger.LogInformation("Found {Count} freshwater publications for page {Page}", result.Count, page);

            return result;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisSection(string sectionType)
        {
            logger.LogDebug("Synchronous GetAllPublicationsInThisSection - Section: {SectionType}", sectionType);

            var publications = this.DbContext.Publications
                .Where(p => p.Section.Name == sectionType)
                .Take(WebConstants.DefaultResultPerPage)
                .ToList();

            var result = GetAllTargetPublications(publications);
            logger.LogInformation("Found {Count} publications in section {SectionType}", result.Count, sectionType);

            return result;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisYear(int year)
        {
            logger.LogDebug("Synchronous GetAllPublicationsInThisYear - Year: {Year}", year);

            var publications = this.DbContext.Publications
                .Where(p => p.CreationDate.Year == year)
                .Take(WebConstants.DefaultResultPerPage)
                .ToList();

            var result = GetAllTargetPublications(publications);
            logger.LogInformation("Found {Count} publications for year {Year}", result.Count, year);

            return result;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisMonth(int month)
        {
            logger.LogDebug("Synchronous GetAllPublicationsInThisMonth - Month: {Month}", month);

            var publications = this.DbContext.Publications
                .Where(p => p.CreationDate.Month == month)
                .Take(WebConstants.DefaultResultPerPage)
                .ToList();

            var result = GetAllTargetPublications(publications);
            logger.LogInformation("Found {Count} publications for month {Month}", result.Count, month);

            return result;
        }

        public int TotalPublicationsCount()
        {
            logger.LogDebug("Synchronous TotalPublicationsCount");

            var count = this.DbContext.Publications.Count();
            logger.LogInformation("Total publications: {Count}", count);

            return count;
        }

        public int TotalPublicationsCount(string type)
        {
            logger.LogDebug("Synchronous TotalPublicationsCount - Type: {Type}", type);

            var count = this.DbContext.Publications
                .Include(p => p.Section)
                .Where(p => p.Section.Name == type)
                .Count();

            logger.LogInformation("Total publications for {Type}: {Count}", type, count);
            return count;
        }

        // ============================================================================
        // PRIVATE HELPER METHODS
        // ============================================================================

        private List<PublicationShortViewModel> GetAllTargetPublications(List<Publication> publications)
        {
            return publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = GetAutorById(p.AuthorId),
                Description = p.Description.GetOnlyTextFromDescription(),
                CoverImage = GetMainImage(p.Description)
            }).ToList();
        }

        private List<PublicationShortViewModel> GetAllTargetPublicationsWithLoadedAuthors(List<Publication> publications)
        {
            return publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = p.Author?.UserName,
                Description = p.Description.GetOnlyTextFromDescription(),
                CoverImage = GetMainImage(p.Description)
            }).ToList();
        }

        private List<Publication> GetTargetSection(string targetSection, int page, int count)
        {
            if (page <= 0) return null;

            return this.DbContext.Publications
                .Where(p => p.Section.Name == targetSection)
                .OrderByDescending(p => p.CreationDate)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();
        }

        private async Task<List<Publication>> GetTargetSectionAsync(string targetSection, int page, int count, CancellationToken cancellationToken = default)
        {
            if (page <= 0) return null;

            return await this.DbContext.Publications
                .Include(p => p.Author)
                .Where(p => p.Section.Name == targetSection)
                .OrderByDescending(p => p.CreationDate)
                .Skip((page - 1) * count)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        private string GetAutorById(string authorId)
        {
            var author = this.DbContext.Users.FirstOrDefault(a => a.Id == authorId);
            return author?.UserName;
        }

        private string GetMainImage(string description)
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;

            var regex = new Regex("<img[^>]+src=\"([^\">]+)\"");
            var matched = regex.Match(description);

            return matched.Success ? matched.Groups[1].Value : string.Empty;
        }
    }
}
