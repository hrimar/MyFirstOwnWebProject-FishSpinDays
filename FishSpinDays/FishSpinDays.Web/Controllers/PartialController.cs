using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Common.Identity.ViewModels;
using FishSpinDays.Services.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FishSpinDays.Web.Controllers
{
    public class PartialController : BaseController
    {
        public PartialController(IBasePublicationsService baseService) 
            : base(baseService)
        { }

        
        public IActionResult _RightBarPartial()
        {
            var model = new SectionShortViewModel()
            {
                Id = 10,
                Name = "Testov"
            };
            return View(model);
        }
    }
}