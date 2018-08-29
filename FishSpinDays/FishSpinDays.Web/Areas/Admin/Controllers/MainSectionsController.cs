namespace FishSpinDays.Web.Areas.Admin.Controllers
{  
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Services.Admin.Interfaces;   
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
                Message = WebConstants.MainSectionCreation
            });
      
            return RedirectToAction("/");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var sectionModel = this.adminSectionService.MainSectionDetails(id);
            if (sectionModel == null)
            {
                return NotFound();
            }
            return View(sectionModel);
        }
    }
}