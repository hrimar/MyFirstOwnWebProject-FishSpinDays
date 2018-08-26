using FishSpinDays.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FishSpinDays.Common.Identity.BindingModels
{
  public  class CommentBindingModel
    {
        [Required]
        [MinLength(WebConstants.NameMinLength)]
        public string Text { get; set; }

        [BindNever]
        public string Author { get; set; }

        [BindNever]
        public DateTime CreationDate { get; set; }        
    }
}
