using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FishSpinDays.Web.Areas.Identity.Pages
{
    public class BaseModel : PageModel
    {
        public BaseModel(FishSpinDaysDbContext dbContex)
        {
            this.DbContext = dbContex;
        }

        public FishSpinDaysDbContext DbContext { get; private set; }
    }
}