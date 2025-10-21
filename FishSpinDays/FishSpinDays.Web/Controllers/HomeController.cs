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

    public class HomeController : BaseController
    {      
        public HomeController(IBasePublicationsService baseService)
            : base(baseService)
        { }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync();

            var publications = await this.BaseService.GetAllPublicationsAsync(id.Value, WebConstants.DefaultResultCount);

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
        
        public async Task<IActionResult> Sea(int? id)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.SeaSection);

            var publications = await this.BaseService.GetAllSeaPublicationsAsync(id.Value, WebConstants.DefaultResultCount);
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

        public async Task<IActionResult> Freshwater(int? id)
        {            
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = await ArrangePagesCountAsync(WebConstants.FreshwaterSection);

            var publications = (await this.BaseService.GetAllFreshwaterPublicationsAsync(id.Value, WebConstants.DefaultResultCount)).ToList();
                      
            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }

        public async Task<IActionResult> Rods()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Rods);
            return View(publications);
        }

        public async Task<IActionResult> Lures()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Lures);
            return View(publications);
        }

        public async Task<IActionResult> Handmade()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.HandLures);
            return View(publications);
        }

        public async Task<IActionResult> Eco()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Eco);
            return View(publications);
        }

        public async Task<IActionResult> School()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.School);
            return View(publications);
        }

        public async Task<IActionResult> Anti()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Anti);
            return View(publications);
        }

        public async Task<IActionResult> Breeding()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisSectionAsync(WebConstants.Breeding);
            return View(publications);
        }

        public async Task<IActionResult> Year()
        {
            var publications = await this.BaseService.GetAllPublicationsInThisYearAsync(WebConstants.Year2018);

            return View(publications);
        }

        public async Task<IActionResult> Month(int id)
        {
			this.ViewData["Month"] = id;
			
            var publications = await this.BaseService.GetAllPublicationsInThisMonthAsync(id);

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

        private async Task<int> ArrangePagesCountAsync()
        {
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync();
            double pages = (totalPublicationsCount / WebConstants.DefaultResultPerTripsPage);
            int requiredPagesForThisPublications = (int)pages;
            if (pages % 1 != 0)
            {
                requiredPagesForThisPublications = requiredPagesForThisPublications + 1;
            }

            return requiredPagesForThisPublications;
        }

        private async Task<int> ArrangePagesCountAsync(string type)
        {
            var totalPublicationsCount = await this.BaseService.TotalPublicationsCountAsync(type);
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
