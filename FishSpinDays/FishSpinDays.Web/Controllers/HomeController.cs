namespace FishSpinDays.Web.Controllers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Web.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Constants;

    public class HomeController : BaseController
    {      
        public HomeController(IBasePublicationsService baseService)
            : base(baseService)
        { }

        [HttpGet]
        public IActionResult Index(int? id)
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

           
            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }
        
        public IActionResult Sea(int? id)
        {
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = ArrangePagesCount(WebConstants.SeaSection);

            var publications = this.BaseService.GetAllSeaPublications(id.Value, 3);
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

        public IActionResult Freshwater(int? id)
        {            
            if (!id.HasValue)
            {
                id = WebConstants.DefaultPage;
            }

            int requiredPagesForThisPublications = ArrangePagesCount(WebConstants.FreshwaterSection);

            var publications = this.BaseService.GetAllFreshwaterPublications(id.Value, 3).ToList();
                      
            return View(new PartPublicationsViewModel()
            {
                Id = id.Value,
                Count = requiredPagesForThisPublications,
                Publications = publications
            });
        }

        public IActionResult Rods()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.Rods);
            return View(publications);
        }

        public IActionResult Lures()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.Lures);
            return View(publications);
        }

        public IActionResult Handmade()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.HandLures);
            return View(publications);
        }

        public IActionResult Eco()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.Eco);
            return View(publications);
        }

        public IActionResult School()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.School);
            return View(publications);
        }

        public IActionResult Anti()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.Anti);
            return View(publications);
        }

        public IActionResult Breeding()
        {
            var publications = this.BaseService.GetAllPublicationsInThisSection(WebConstants.Breeding);
            return View(publications);
        }

        public IActionResult Year()
        {
            var publications = this.BaseService.GetAllPublicationsInThisYear(WebConstants.Year2018);

            return View(publications);
        }

        public IActionResult Month(int id)
        {
            var publications = this.BaseService.GetAllPublicationsInThisMonth(id);

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
