namespace FishSpinDays.Web.Controllers
{    
    using FishSpinDays.Services.Base.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class PublicationsController : BaseController
    {
        public PublicationsController(IBasePublicationsService baseService) 
            : base(baseService)
        {  }

        public IActionResult Details(int id)
        {
            var publicationModel = this.BaseService.GetPublication(id);
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }

        public IActionResult MostRated()
        {
            var publicationModel = this.BaseService.MostReaded();
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }
    }
}