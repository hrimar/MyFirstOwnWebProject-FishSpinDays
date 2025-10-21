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

namespace FishSpinDays.Web.Controllers
{
    [Route("api/publications")] // for url: http://localhost:51034/api/publications
    [ApiController]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PublicationsAPIController : BaseAPIController
    {
        private readonly UserManager<User> userManager;
        private readonly IIdentityService identityService;

        public PublicationsAPIController(UserManager<User> userManager,
            IIdentityService identityService, IBasePublicationsService baseService)
            : base(baseService)
        {
            this.userManager = userManager;
            this.identityService = identityService;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<ActionResult<PartPublicationsViewModel>> GetAllPublications(int? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);

            var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, 3, cancellationToken);

            if (publications == null)
            {
                return NotFound();
            }

            return new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            };
        }

        [HttpGet("{id}", Name = "Details")] // Details will be the name of this method and with this name will be used on row.99!
        [AllowAnonymous]
        public async Task<IActionResult> GetPublication(int id, CancellationToken cancellationToken = default) 
        {
            var publicationModel = await this.BaseService.GetPublicationAsync(id, cancellationToken);
            if (publicationModel == null)
            {
                return NotFound(new { Message = "The publication does not exist." }); // TODO:  Put in constants!
            }

            return Ok(publicationModel);
        }
                
        [HttpPost("")]
        [Authorize]  // To use this for API makes AuthControler
        public async Task<IActionResult> CreatePublication([FromBody]PublicationBindingModel model, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            User author = await this.userManager.GetUserAsync(this.User);
            var section = await this.identityService.GetSectionByNameAsync(model.Section, cancellationToken);

            if (author == null || section == null)
            {
                return NotFound();
            }

            var publication = await this.identityService.CreatePublicationAsync(author, section, model.Title, model.Description, cancellationToken);


            return CreatedAtAction("Details", new { id = publication.Id });
        }

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
