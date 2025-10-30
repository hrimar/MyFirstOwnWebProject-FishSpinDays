namespace FishSpinDays.Web.Controllers.API
{
    using System;
    using System.Threading.Tasks;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using FishSpinDays.Web.Helpers.Filters;
    using System.Linq;

    /// <summary>
    /// API Controller for sections management
    /// </summary>
    [Route("api/sections")]
    [ApiController]
[IgnoreAntiforgeryToken]
    [ApiSecurityValidation]
    public class SectionsAPIController : ControllerBase
    {
   private readonly IIdentityService identityService;
        private readonly ILogger<SectionsAPIController> logger;

        public SectionsAPIController(
            IIdentityService identityService,
            ILogger<SectionsAPIController> logger)
{
            this.identityService = identityService;
            this.logger = logger;
        }

        /// <summary>
        /// Get all available sections
        /// </summary>
    [HttpGet("")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetAllSections")]
        public async Task<IActionResult> GetAllSections(CancellationToken cancellationToken = default)
        {
   try
            {
    var sections = await identityService.GetAllSectionsAsync(cancellationToken);

             var sectionsList = sections.Select(s => new
       {
            Id = int.Parse(s.Value),
     Name = s.Text
       }).ToList();

    return Ok(sectionsList);
         }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
          {
throw;
   }
            catch (Exception ex)
{
        logger.LogError(ex, "Error getting all sections");
                return StatusCode(500, new { Message = "An error occurred while retrieving sections." });
        }
        }

/// <summary>
        /// Get section by ID
        /// </summary>
        [HttpGet("{id}")]
      [AllowAnonymous]
      [SimpleOperation(OperationName = "GetSection")]
        public async Task<IActionResult> GetSection(int id, CancellationToken cancellationToken = default)
        {
       try
        {
 var section = await identityService.GetSectionAsync(id, cancellationToken);

      if (section == null)
           {
      return NotFound(new { Message = "Section not found." });
           }

    return Ok(new
       {
         Id = id,
      Name = section.Name
    });
         }
       catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
   throw;
       }
   catch (Exception ex)
     {
        logger.LogError(ex, "Error getting section - ID: {SectionId}", id);
                return StatusCode(500, new { Message = "An error occurred while retrieving the section." });
          }
        }

        /// <summary>
        /// Get section by name
        /// </summary>
  [HttpGet("by-name/{name}")]
        [AllowAnonymous]
        [SimpleOperation(OperationName = "GetSectionByName")]
        public async Task<IActionResult> GetSectionByName(string name, CancellationToken cancellationToken = default)
 {
            try
            {
             if (string.IsNullOrWhiteSpace(name))
    {
    return BadRequest(new { Message = "Section name cannot be empty." });
                }

          var section = await identityService.GetSectionByNameAsync(name, cancellationToken);
  
       if (section == null)
   {
    return NotFound(new { Message = "Section not found." });
    }

         return Ok(new
   {
 Id = section.Id,
  Name = section.Name
                });
         }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
      {
   throw;
     }
      catch (Exception ex)
            {
 logger.LogError(ex, "Error getting section by name - Name: {SectionName}", name);
                return StatusCode(500, new { Message = "An error occurred while retrieving the section." });
            }
    }
    }
}