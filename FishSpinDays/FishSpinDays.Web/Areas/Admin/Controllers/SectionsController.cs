using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Common.Admin.BindingModels;
using FishSpinDays.Services.Admin.Interfaces;
using FishSpinDays.Services.Identity;
using FishSpinDays.Services.Identity.Interfaces;
using FishSpinDays.Web.Helpers;
using FishSpinDays.Web.Helpers.Messages;
using Microsoft.AspNetCore.Mvc;

namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    public class SectionsController : AdminController
    {
        private readonly IAdminSectionsService adminSectionService;
     

        public SectionsController( IAdminSectionsService adminSectionService) 
        {
           this.adminSectionService = adminSectionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var modelSection = this.adminSectionService.GetSections();

            return View(modelSection);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = this.adminSectionService.PrepareSectionForCreation(id); //mainSectionId
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
            //return View();
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]       
        public  IActionResult Create(CreateSectionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
                       
            var section = this.adminSectionService.AddSection(model);

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = "The section was created succesfully."
            });

            //return RedirectToAction("Details", new { id = section.Id });           
            return RedirectToAction("/");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {           
            var courseModel =  this.adminSectionService.SectionDetails(id);
            return View(courseModel);
        }
    }
}