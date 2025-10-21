namespace FishSpinDays.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Web.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Constants;
    using System.Threading.Tasks;
    using System.Threading;

    public class HomeController : BaseController
    {      
        public HomeController(IBasePublicationsService baseService)
            : base(baseService)
        { }

        [HttpGet]
        public async Task<IActionResult> Index(int? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(cancellationToken);

            var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken);

            if (publications == null)
            {
                return NotFound();
            }

            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }
        
        public async Task<IActionResult> Sea(int? id, CancellationToken cancellationToken = default)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.SeaSection, cancellationToken);

            var publications = await this.BaseService.GetAllSeaPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken);
            if (publications == null)
            {
                return NotFound();
            }

            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }

        public async Task<IActionResult> Freshwater(int? id, CancellationToken cancellationToken = default)
        {            
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.FreshwaterSection, cancellationToken);

            var publications = (await this.BaseService.GetAllFreshwaterPublicationsAsync(id.Value, WebConstants.DefaultResultCount, cancellationToken)).ToList();
                      
            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }

        public async Task<IActionResult> Rods(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Rods, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Lures(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Lures, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Handmade(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.HandLures, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Eco(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Eco, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> School(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.School, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Anti(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Anti, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Breeding(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Breeding, cancellationToken);
            return View(publications);
        }

        public async Task<IActionResult> Year(CancellationToken cancellationToken = default)
        {
            var publications = await this.BaseService.GetAllPublicationsInThisYearAsync(WebConstants.Year2018, cancellationToken);

            return View(publications);
        }

        public async Task<IActionResult> Month(int id, CancellationToken cancellationToken = default)
        {
			this.ViewData["Month"] = id;
			
            var publications = await this.BaseService.GetAllPublicationsInThisMonthAsync(id, cancellationToken);

            return View(publications);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Info()
        {
            return View();
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

        private async Task<int> ArrangePagesCountAsync(string type, CancellationToken cancellationToken = default)
        {
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(type, cancellationToken);
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
