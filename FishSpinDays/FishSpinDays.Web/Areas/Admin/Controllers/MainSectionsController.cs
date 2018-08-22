namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class MainSectionsController : AdminController
    {
        private readonly IAdminSectionsService adminSectionService;
       
        public MainSectionsController(IAdminSectionsService adminSectionService)
        {
            this.adminSectionService = adminSectionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var modelMainSection = this.adminSectionService.GetMainSections();

            return View(modelMainSection);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]       
        public IActionResult Create(CreateMainSectionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var mainSection = this.adminSectionService.AddMainSection(model);

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = "The main section was created succesfully."
            });

            ////return RedirectToAction("Details", new { id = section.Id });           
            return RedirectToAction("/");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var courseModel = this.adminSectionService.MainSectionDetails(id); 
            return View(courseModel);
        }
    }
}