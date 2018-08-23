using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FishSpinDays.Common.Admin.BindingModels
{
   public class BanBindingModel
    {
        public string Id { get; set; }

        [Display(Name = "Block until")]      
        public DateTime LockoutEnd { get; set; }
    }
}
