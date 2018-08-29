namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class PublicationsController : AdminController
    {
        private readonly IAdminPublicationsService adminPublicationsService;


        public PublicationsController(IAdminPublicationsService adminSectionService)
        {
            this.adminPublicationsService = adminSectionService;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var publicationModel = this.adminPublicationsService.GetPublication(id);
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }

        [HttpPost]
        public IActionResult Edit(PublicationBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var publication = this.adminPublicationsService.EditPublication(model);
            if (publication == null)
            {
                return NotFound();
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = WebConstants.PublicationEditSuccess
            });

            return Redirect($"/Publications/Details/{model.Id}");           
        }
    }
}