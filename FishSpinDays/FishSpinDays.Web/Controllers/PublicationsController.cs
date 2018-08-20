using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Services.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FishSpinDays.Web.Controllers
{
    public class PublicationsController : BaseController
    {
        public PublicationsController(IBasePublicationsService baseService) 
            : base(baseService)
        {  }

        public IActionResult Details(int id)
        {
            var publicationModel = this.BaseService.GetPublication(id);
            return View(publicationModel);
        }
    }
}