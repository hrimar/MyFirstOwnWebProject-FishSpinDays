using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult<PartPublicationsViewModel> GetAllPublications(int? id)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = ArrangePagesCount();

            var publications = this.BaseService.GetAllPublications(id.Value, 3);

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
        public IActionResult GetPublication(int id) 
        {
            var publicationModel = this.BaseService.GetPublication(id);
            if (publicationModel == null)
            {
                return NotFound(new { Message = "The publication does not exist." }); // TODO:  Put in constants!
            }

            return Ok(publicationModel);
        }
                
        [HttpPost("")]
        [Authorize]  // To use this for API makes AuthControler
        public IActionResult CreatePublication([FromBody]PublicationBindingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            User author = this.userManager.GetUserAsync(this.User).Result;
            var section = this.identityService.GetSectionByName(model.Section);

            if (author == null || section == null)
            {
                return NotFound();
            }

            var publication = this.identityService.CreatePublication(author, section, model.Title, model.Description);


            return CreatedAtAction("Details", new { id = publication.Id });
        }



        private int ArrangePagesCount()
        {
            var totalPublicationsCount = this.BaseService.TotalPublicationsCount();
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            return requiredPagesForThisPublications;
        }

        private int ArrangePagesCount(string type)
        {
            var totalPublicationsCount = this.BaseService.TotalPublicationsCount(type);
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
