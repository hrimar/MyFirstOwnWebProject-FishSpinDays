namespace FishSpinDays.Web.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using FishSpinDays.Common.Base.ViewModels;
 using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using FishSpinDays.Web.Helpers.Filters;
  using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Linq;
    using FishSpinDays.Common.API.Models.Search;

    /// <summary>
    /// API Controller for search functionality
    /// </summary>
    [Route("api/search")]
    [ApiController]
    [IgnoreAntiforgeryToken]
    [ApiSecurityValidation]
    public class SearchAPIController : ControllerBase
    {
        private readonly IIdentityService identityService;
        private readonly ILogger<SearchAPIController> logger;

        public SearchAPIController(
            IIdentityService identityService,
            ILogger<SearchAPIController> logger)
        {
        this.identityService = identityService;
this.logger = logger;
        }

        /// <summary>
        /// Search publications by term with highlighting
        /// </summary>
      [HttpGet("publications")]
        [AllowAnonymous]
      [ImportantOperation(OperationName = "SearchPublications")]
        public async Task<ActionResult<SearchResultsModel>> SearchPublications(
      [FromQuery] string searchTerm,
   [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
       try
            {
            // Validation
      if (string.IsNullOrWhiteSpace(searchTerm))
          {
      return BadRequest(new { Message = "Search term cannot be empty." });
           }

              if (searchTerm.Length < 2)
      {
 return BadRequest(new { Message = "Search term must be at least 2 characters long." });
                }

    if (searchTerm.Length > 100)
  {
       return BadRequest(new { Message = "Search term is too long. Maximum 100 characters allowed." });
       }

     if (page < 1) page = 1;
     if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var foundPublications = await identityService.FoundPublicationsAsync(searchTerm, cancellationToken);

 // Highlight search terms in results
  var highlightedResults = foundPublications.Select(result => new SearchPublicationViewModel
    {
      Id = result.Id,
     Title = result.Title,
     SearchResult = HighlightSearchTerm(result.SearchResult, searchTerm)
           }).ToList();

       // Implement basic pagination
     var totalResults = highlightedResults.Count;
           var totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
 var pagedResults = highlightedResults.Skip((page - 1) * pageSize).Take(pageSize).ToList();

  var searchResults = new SearchResultsModel
           {
        SearchTerm = searchTerm,
         TotalResults = totalResults,
           Page = page,
                    PageSize = pageSize,
           TotalPages = totalPages,
       Results = pagedResults
   };

    return Ok(searchResults);
        }
     catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
       {
      throw;
       }
            catch (Exception ex)
            {
       logger.LogError(ex, "Error searching publications - SearchTerm: {SearchTerm}", searchTerm);
   return StatusCode(500, new { Message = "An error occurred while searching publications." });
            }
        }

    /// <summary>
        /// Get search suggestions based on partial input
        /// </summary>
        [HttpGet("suggestions")]
      [AllowAnonymous]
        [SimpleOperation(OperationName = "GetSearchSuggestions")]
        public async Task<ActionResult<List<string>>> GetSearchSuggestions([FromQuery] string term, CancellationToken cancellationToken = default)
        {
     try
{
          if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
      {
                return Ok(new List<string>());
                }

      // This is a basic implementation. In a real-world scenario,
     // we'd want to use a more sophisticated search index or pre-computed suggestions
         var foundPublications = await identityService.FoundPublicationsAsync(term, cancellationToken);

  var suggestions = foundPublications
    .Take(5) // Limit to 5 suggestions
    .Select(p => p.Title)
        .Distinct()
      .ToList();

        return Ok(suggestions);
          }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
         {
        throw;
            }
      catch (Exception ex)
            {
             logger.LogError(ex, "Error getting search suggestions - Term: {Term}", term);
       return StatusCode(500, new { Message = "An error occurred while getting search suggestions." });
            }
     }

 /// <summary>
        /// Advanced search with filters
      /// </summary>
        [HttpPost("advanced")]
      [AllowAnonymous]
        [ImportantOperation(OperationName = "AdvancedSearch")]
    public async Task<ActionResult<SearchResultsModel>> AdvancedSearch([FromBody] AdvancedSearchModel searchModel, CancellationToken cancellationToken = default)
        {
            try
   {
           if (!ModelState.IsValid)
     {
        return BadRequest(ModelState);
        }

            var foundPublications = await identityService.FoundPublicationsAsync(searchModel.SearchTerm, cancellationToken);

        // Apply additional filters based on the advanced search model
         var filteredResults = foundPublications.AsQueryable();

    // For now, we'll just use the basic search functionality

        var highlightedResults = filteredResults.Select(result => new SearchPublicationViewModel
       {
   Id = result.Id,
          Title = result.Title,
             SearchResult = HighlightSearchTerm(result.SearchResult, searchModel.SearchTerm)
           }).ToList();

            var searchResults = new SearchResultsModel
  {
   SearchTerm = searchModel.SearchTerm,
   TotalResults = highlightedResults.Count,
        Page = searchModel.Page,
            PageSize = searchModel.PageSize,
TotalPages = (int)Math.Ceiling(highlightedResults.Count / (double)searchModel.PageSize),
         Results = highlightedResults
     .Skip((searchModel.Page - 1) * searchModel.PageSize)
    .Take(searchModel.PageSize)
    .ToList()
    };

            return Ok(searchResults);
            }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
throw;
   }
            catch (Exception ex)
 {
                logger.LogError(ex, "Error in advanced search - SearchTerm: {SearchTerm}", searchModel?.SearchTerm);
     return StatusCode(500, new { Message = "An error occurred during advanced search." });
   }
    }

        private static string HighlightSearchTerm(string text, string searchTerm)
        {
         if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
       return text;

            return Regex.Replace(
       text,
   $"({Regex.Escape(searchTerm)})",
            match => $"<mark>{match.Groups[0].Value}</mark>",
       RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}