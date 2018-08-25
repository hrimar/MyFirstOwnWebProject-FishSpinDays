using FishSpinDays.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FishSpinDays.Models
{
   public class MainSection
    {
        public MainSection()
        {
            this.Sections = new List<Section>();
        }

        public int Id { get; set; }

        public ICollection<Section> Sections { get; set; }

        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Name { get; set; }

    }
}
