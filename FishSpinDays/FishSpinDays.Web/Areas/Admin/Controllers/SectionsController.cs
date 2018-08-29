namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;

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
            var modelSections = this.adminSectionService.GetSections();
            if (modelSections == null)
            {
                return NotFound();
            }

            return View(modelSections);
        }

        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = this.adminSectionService.PrepareSectionForCreation(id); 
            if (model == null)
            {
                return NotFound();
            }

            return View(model);           
        }

        [HttpPost]    
        public  IActionResult Create(CreateSectionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
                       
            var section = this.adminSectionService.AddSection(model);
            if (section == null)
            {
                return NotFound();
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = WebConstants.SectionCreation
            });
                             
            return RedirectToAction("/");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {           
            var sectionModel =  this.adminSectionService.SectionDetails(id);
            if (sectionModel == null)
            {
                return NotFound();
            }

            return View(sectionModel);
        }
    }
}