namespace FishSpinDays.Web.Controllers
{    
    using FishSpinDays.Services.Base.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class PublicationsController : BaseController
    {
        public PublicationsController(IBasePublicationsService baseService) 
            : base(baseService)
        {  }

        public async Task<IActionResult> Details(int id)
        {
            var publicationModel = await this.BaseService.GetPublicationAsync(id);
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }

        public async Task<IActionResult> MostRated()
        {
            var publicationModel = await this.BaseService.MostReadedAsync();
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }
    }
}