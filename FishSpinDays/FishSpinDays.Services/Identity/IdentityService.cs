namespace FishSpinDays.Services.Identity
{
    using AutoMapper;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FishSpinDays.Common.Extensions;
    using System.Threading.Tasks;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;

    public class IdentityService : BaseService, IIdentityService
    {
        private readonly ILogger<IdentityService> logger;
        
        public IdentityService(FishSpinDaysDbContext dbContex, IMapper mapper, ILogger<IdentityService> logger) 
            : base(dbContex, mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================================
        // CRITICAL OPERATIONS - Full Logging with Performance Monitoring
        // ============================================================================

        public async Task<Publication> CreatePublicationAsync(User author, Section section, string title, string description, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("CreatePublication - AuthorId: {AuthorId}, SectionId: {SectionId}, Title: {Title}", author?.Id, section?.Id, title);
            using var activity = new Activity("CreatePublication");
            activity.Start();
            
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async CreatePublication operation");

            Publication publication = new Publication()
            {
                Author = author,
                Section = section,
                Title = title,
                Description = description
            };

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("CreatePublication was cancelled before database save - Title: {Title}", title);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Adding publication to database context");
                this.DbContext.Publications.Add(publication);
                await this.DbContext.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Publication created successfully after {ElapsedMs}ms - ID: {PublicationId}, Title: {Title}, AuthorId: {AuthorId}", 
                    stopwatch.ElapsedMilliseconds, publication.Id, title, author?.Id);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Slow CreatePublication operation - took {ElapsedMs}ms for title: {Title}", 
                        stopwatch.ElapsedMilliseconds, title);
                }

                return publication;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("CreatePublication was cancelled after {ElapsedMs}ms - Title: {Title}", 
                    stopwatch.ElapsedMilliseconds, title);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Failed to create publication after {ElapsedMs}ms - Title: {Title}, AuthorId: {AuthorId}, SectionId: {SectionId}", 
                    stopwatch.ElapsedMilliseconds, title, author?.Id, section?.Id);
                return null;
            }
            finally
            {
                activity.Stop();
            }
        }

        public async Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetUserById - ID: {UserId}", id);
            using var activity = new Activity("GetUserById");
            activity.Start();

            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting async GetUserById operation with full loading");

            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("GetUserById was cancelled before database query - ID: {UserId}", id);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                logger.LogDebug("Executing database query with eager loading for publications and comments");
                var user = await this.DbContext.Users
                    .Include(u => u.Publications)
                    .ThenInclude(u => u.Comments)
                    .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

                stopwatch.Stop();

                if (user == null)
                {
                    logger.LogWarning("User not found after {ElapsedMs}ms - ID: {UserId}", stopwatch.ElapsedMilliseconds, id);
                }
                else
                {
                    logger.LogDebug("User found after {ElapsedMs}ms - ID: {UserId}, UserName: {UserName}, Publications: {PublicationCount}", 
                        stopwatch.ElapsedMilliseconds, id, user.UserName, user.Publications?.Count ?? 0);
                }

                logger.LogInformation("GetUserById completed - ID: {UserId}, Found: {Found}, Duration: {ElapsedMs}ms", 
                    id, user != null, stopwatch.ElapsedMilliseconds);

                // Log slow operations
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    logger.LogWarning("Slow GetUserById operation - ID: {UserId} took {ElapsedMs}ms", id, stopwatch.ElapsedMilliseconds);
                }

                return user;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetUserById was cancelled after {ElapsedMs}ms - ID: {UserId}", stopwatch.ElapsedMilliseconds, id);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetUserById after {ElapsedMs}ms - ID: {UserId}", stopwatch.ElapsedMilliseconds, id);
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

        public async Task<Section> GetSectionAsync(int id, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("GetSection - ID: {SectionId}", id);
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting GetSection operation");

            try
            {
                var section = await this.DbContext.Sections.FindAsync(new object[] { id }, cancellationToken);
                
                stopwatch.Stop();
                logger.LogInformation("GetSection completed - ID: {SectionId}, Found: {Found}, Duration: {ElapsedMs}ms", 
                    id, section != null, stopwatch.ElapsedMilliseconds);

                return section;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("GetSection was cancelled after {ElapsedMs}ms - ID: {SectionId}", stopwatch.ElapsedMilliseconds, id);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in GetSection after {ElapsedMs}ms - ID: {SectionId}", stopwatch.ElapsedMilliseconds, id);
                throw;
            }
        }

        public async Task<Comment> AddCommentAsync(User author, Publication publication, string text, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("AddComment - PublicationId: {PublicationId}, AuthorId: {AuthorId}", publication?.Id, author?.Id);
            
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting AddComment operation - Comment length: {TextLength}", text?.Length ?? 0);

            Comment comment = new Comment()
            {
                Author = author,
                Publication = publication,
                Text = text
            };

            try
            {
                this.DbContext.Comments.Add(comment);
                await this.DbContext.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Comment added successfully after {ElapsedMs}ms - ID: {CommentId}, PublicationId: {PublicationId}, AuthorId: {AuthorId}", 
                    stopwatch.ElapsedMilliseconds, comment.Id, publication?.Id, author?.Id);

                return comment;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("AddComment operation was cancelled after {ElapsedMs}ms - PublicationId: {PublicationId}", 
                    stopwatch.ElapsedMilliseconds, publication?.Id);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Failed to add comment after {ElapsedMs}ms - PublicationId: {PublicationId}, AuthorId: {AuthorId}", 
                    stopwatch.ElapsedMilliseconds, publication?.Id, author?.Id);
                return null;
            }
        }

        public async Task<List<SearchPublicationViewModel>> FoundPublicationsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            using var scope = logger.BeginScope("FoundPublications - SearchTerm: {SearchTerm}", searchTerm);
            var stopwatch = Stopwatch.StartNew();
            logger.LogDebug("Starting search publications operation - Term length: {TermLength}", searchTerm?.Length ?? 0);

            try
            {
                var foundPublications = await this.DbContext.Publications
                   .Where(a => a.Description.ToLower().Contains(searchTerm.ToLower()))
                   .OrderBy(a => a.CreationDate)
                   .Select(a => new SearchPublicationViewModel()
                   {
                       Id = a.Id,
                       SearchResult = a.Description.GetOnlyTextFromDescription(),
                       Title = a.Title
                   })
                   .ToListAsync(cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("Search completed after {ElapsedMs}ms - SearchTerm: {SearchTerm}, Results: {ResultCount}", 
                    stopwatch.ElapsedMilliseconds, searchTerm, foundPublications.Count);

                return foundPublications;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                stopwatch.Stop();
                logger.LogInformation("Search publications operation was cancelled after {ElapsedMs}ms - SearchTerm: {SearchTerm}", 
                    stopwatch.ElapsedMilliseconds, searchTerm);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                logger.LogError(ex, "Error in search publications after {ElapsedMs}ms - SearchTerm: {SearchTerm}", 
                    stopwatch.ElapsedMilliseconds, searchTerm);
                throw;
            }
        }

        // ============================================================================
        // SIMPLE OPERATIONS - Minimal Logging
        // ============================================================================

        public async Task<Section> GetSectionByNameAsync(string sectionName, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting section by name: {SectionName}", sectionName);

            try
            {
                var section = await this.DbContext.Sections.FirstOrDefaultAsync(s => s.Name == sectionName, cancellationToken);
                logger.LogInformation("Section by name {SectionName} {Status}", sectionName, section != null ? "found" : "not found");
                return section;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetSectionByName cancelled - Name: {SectionName}", sectionName);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting section by name: {SectionName}", sectionName);
                throw;
            }
        }

        public async Task<Publication> GetPublicationByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting publication by ID: {PublicationId}", id);

            try
            {
                var publication = await this.DbContext.Publications.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                logger.LogInformation("Publication {PublicationId} {Status}", id, publication != null ? "found" : "not found");
                return publication;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetPublicationById cancelled - ID: {PublicationId}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting publication by ID: {PublicationId}", id);
                throw;
            }
        }

        public async Task<Comment> GetCommentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting comment by ID: {CommentId}", id);

            try
            {
                var comment = await this.DbContext.Comments.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
                logger.LogInformation("Comment {CommentId} {Status}", id, comment != null ? "found" : "not found");
                return comment;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetCommentById cancelled - ID: {CommentId}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting comment by ID: {CommentId}", id);
                throw;
            }
        }

        public async Task<bool> IsLikedPublicationAsync(Publication publication, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Liking publication - ID: {PublicationId}, Current likes: {CurrentLikes}", publication?.Id, publication?.Likes);

            try
            {
                publication.Likes++;
                await this.DbContext.SaveChangesAsync(cancellationToken);
                
                logger.LogInformation("Publication liked - ID: {PublicationId}, New likes: {LikesCount}", 
                    publication.Id, publication.Likes);
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Like publication cancelled - ID: {PublicationId}", publication?.Id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to like publication - ID: {PublicationId}", publication?.Id);
                return false;
            }
        }

        public async Task<bool> IsLikedCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Liking comment - ID: {CommentId}", comment?.Id);

            try
            {
                comment.Likes++;
                await this.DbContext.SaveChangesAsync(cancellationToken);
                
                logger.LogInformation("Comment liked - ID: {CommentId}, New likes: {LikesCount}", 
                    comment.Id, comment.Likes);
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Like comment cancelled - ID: {CommentId}", comment?.Id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to like comment - ID: {CommentId}", comment?.Id);
                return false;
            }
        }

        public async Task<bool> IsUnLikedCommentAsync(Comment comment, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Unliking comment - ID: {CommentId}", comment?.Id);

            try
            {
                comment.UnLikes++;
                await this.DbContext.SaveChangesAsync(cancellationToken);
                
                logger.LogInformation("Comment unliked - ID: {CommentId}, New unlikes: {UnlikesCount}", comment.Id, comment.UnLikes);
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Unlike comment cancelled - ID: {CommentId}", comment?.Id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to unlike comment - ID: {CommentId}", comment?.Id);
                return false;
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAllSectionsAsync(CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Getting all sections for select list");

            try
            {
                var sections = await this.DbContext.Sections
                    .Select(b => new SelectListItem()
                    {
                        Text = b.Name,
                        Value = b.Id.ToString()
                    })
                    .ToListAsync(cancellationToken);

                logger.LogInformation("Retrieved {SectionCount} sections for select list", sections.Count);
                return sections;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("GetAllSections cancelled");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting all sections");
                throw;
            }
        }

        // ============================================================================
        // SYNCHRONOUS METHODS - Basic Logging (Legacy Support)
        // ============================================================================

        public Section GetSection(int id)
        {
            logger.LogDebug("Synchronous GetSection - ID: {SectionId}", id);

            var section = this.DbContext.Sections.Find(id);
            logger.LogInformation("Section {SectionId} {Status}", id, section != null ? "found" : "not found");
            
            return section;
        }

        public Section GetSectionByName(string sectionName)
        {
            logger.LogDebug("Synchronous GetSectionByName - Name: {SectionName}", sectionName);

            var section = this.DbContext.Sections.FirstOrDefault(s => s.Name == sectionName);
            logger.LogInformation("Section by name {SectionName} {Status}", sectionName, section != null ? "found" : "not found");
            
            return section;
        }

        public Publication CreatePublication(User author, Section section, string title, string description)
        {
            logger.LogDebug("Synchronous CreatePublication - Title: {Title}", title);

            Publication publication = new Publication()
            {
                Author = author,
                Section = section,
                Title = title,
                Description = description
            };

            try
            {
                this.DbContext.Publications.Add(publication);
                this.DbContext.SaveChanges();

                logger.LogInformation("Publication created - ID: {PublicationId}, Title: {Title}", publication.Id, title);
                return publication;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create publication - Title: {Title}", title);
                return null;
            }
        }

        public Publication GetPublicationById(int id)
        {
            logger.LogDebug("Synchronous GetPublicationById - ID: {PublicationId}", id);

            var publication = this.DbContext.Publications.FirstOrDefault(p => p.Id == id);
            logger.LogInformation("Publication {PublicationId} {Status}", id, publication != null ? "found" : "not found");
            
            return publication;
        }

        public bool IsLikedPublication(Publication publication)
        {
            logger.LogDebug("Synchronous like publication - ID: {PublicationId}", publication?.Id);

            try
            {
                publication.Likes++;
                this.DbContext.SaveChanges();
                logger.LogInformation("Publication liked - ID: {PublicationId}, New likes: {LikesCount}", publication.Id, publication.Likes);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to like publication - ID: {PublicationId}", publication?.Id);
                return false;
            }
        }

        public Comment AddComment(User author, Publication publication, string text)
        {
            logger.LogDebug("Synchronous AddComment - PublicationId: {PublicationId}", publication?.Id);

            Comment comment = new Comment()
            {
                Author = author,
                Publication = publication,
                Text = text
            };

            try
            {
                this.DbContext.Comments.Add(comment);
                this.DbContext.SaveChanges();

                logger.LogInformation("Comment added - ID: {CommentId}, PublicationId: {PublicationId}", comment.Id, publication?.Id);
                return comment;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to add comment - PublicationId: {PublicationId}", publication?.Id);
                return null;
            }
        }

        public Comment GetCommentById(int id)
        {
            logger.LogDebug("Synchronous GetCommentById - ID: {CommentId}", id);

            var comment = this.DbContext.Comments.FirstOrDefault(p => p.Id == id);
            logger.LogInformation("Comment {CommentId} {Status}", id, comment != null ? "found" : "not found");
            
            return comment;
        }

        public bool IsLikedComment(Comment comment)
        {
            logger.LogDebug("Synchronous like comment - ID: {CommentId}", comment?.Id);

            try
            {
                comment.Likes++;
                this.DbContext.SaveChanges();
                logger.LogInformation("Comment liked - ID: {CommentId}, New likes: {LikesCount}", comment.Id, comment.Likes);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to like comment - ID: {CommentId}", comment?.Id);
                return false;
            }
        }

        public bool IsUnLikedComment(Comment comment)
        {
            logger.LogDebug("Synchronous unlike comment - ID: {CommentId}", comment?.Id);

            try
            {
                comment.UnLikes++;
                this.DbContext.SaveChanges();
                logger.LogInformation("Comment unliked - ID: {CommentId}, New unlikes: {UnlikesCount}", comment.Id, comment.UnLikes);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to unlike comment - ID: {CommentId}", comment?.Id);
                return false;
            }
        }

        public IEnumerable<SelectListItem> GettAllSections()
        {
            logger.LogDebug("Synchronous GetAllSections");

            var sections = this.DbContext.Sections
                .Select(b => new SelectListItem()
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                })
                .ToList();

            logger.LogInformation("Retrieved {SectionCount} sections", sections.Count);
            return sections;
        }

        public User GetUserById(string id)
        {
            logger.LogDebug("Synchronous GetUserById - ID: {UserId}", id);

            var user = this.DbContext.Users
                .Include(u => u.Publications)
                .ThenInclude(u => u.Comments)
                .FirstOrDefault(u => u.Id == id);

            logger.LogInformation("User {UserId} {Status}", id, user != null ? "found" : "not found");
            return user;
        }

        public List<SearchPublicationViewModel> FoundPublications(string searchTerm)
        {
            logger.LogDebug("Synchronous search publications - SearchTerm: {SearchTerm}", searchTerm);

            var foundPublications = this.DbContext.Publications
               .Where(a => a.Description.ToLower().Contains(searchTerm.ToLower()))
               .OrderBy(a => a.CreationDate)
               .Select(a => new SearchPublicationViewModel()
               {
                   Id = a.Id,
                   SearchResult = a.Description.GetOnlyTextFromDescription(),
                   Title = a.Title
               })
               .ToList();

            logger.LogInformation("Search completed - SearchTerm: {SearchTerm}, Results: {ResultCount}", searchTerm, foundPublications.Count);
            return foundPublications;
        }
    }
}
