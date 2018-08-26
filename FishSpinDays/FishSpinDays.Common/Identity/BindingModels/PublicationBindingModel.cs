namespace FishSpinDays.Common.Identity.BindingModels
{
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PublicationBindingModel
    {
        public PublicationBindingModel()
        {
            this.Sections = new List<SelectListItem>();
            this.CreationDate = DateTime.Now;
        }

        [Required(ErrorMessage = "You have to specify a section.")]
        [Display(Name = "Section")]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public int SectionId { get; set; }

        public IEnumerable<SelectListItem> Sections { get; set; }

        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Title { get; set; }

        [MinLength(WebConstants.ContentMinLength, ErrorMessage = ValidationConstants.DescriptionNameMessage)]
        [Required(ErrorMessage = ValidationConstants.DescriptionNullMessage)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public string Author { get; set; }
    }
}
