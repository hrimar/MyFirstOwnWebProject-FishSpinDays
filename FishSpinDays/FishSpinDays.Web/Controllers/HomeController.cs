using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FishSpinDays.Web.Models;
using FishSpinDays.Data;
using FishSpinDays.Services.Base.Interfaces;

namespace FishSpinDays.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBasePublicationsService baseService;

        public HomeController(IBasePublicationsService baseService)
        {
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            var publications = this.baseService.GetAllPublications();

            return View(publications);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
    }
}
