using FishSpinDays.Services.Base.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FishSpinDays.Web.Controllers
{
    public class BaseAPIController : ControllerBase
    {
        public BaseAPIController(IBasePublicationsService baseService)
        {
            this.BaseService = baseService;
        }

        public IBasePublicationsService BaseService { get; private set; }
    }
}
