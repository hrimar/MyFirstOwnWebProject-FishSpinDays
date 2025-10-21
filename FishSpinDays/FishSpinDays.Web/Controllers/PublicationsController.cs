namespace FishSpinDays.Web.Controllers
{    
    using FishSpinDays.Services.Base.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using System.Threading;

    public class PublicationsController : BaseController
    {
        public PublicationsController(IBasePublicationsService baseService) 
            : base(baseService)
        {  }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            var publicationModel = await this.BaseService.GetPublicationAsync(id, cancellationToken);
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }

        public async Task<IActionResult> MostRated(CancellationToken cancellationToken = default)
        {
            var publicationModel = await this.BaseService.MostReadedAsync(cancellationToken);
            if (publicationModel == null)
            {
                return NotFound();
            }

            return View(publicationModel);
        }
    }
}