using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Services.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FishSpinDays.Web.Controllers
{
    public class BaseController : Controller
    {       
        public BaseController(IBasePublicationsService baseService)
        {
            this.BaseService = baseService;
        }

        public IBasePublicationsService BaseService { get; private set; }
    }
}