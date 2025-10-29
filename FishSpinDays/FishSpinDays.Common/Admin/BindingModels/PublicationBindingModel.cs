namespace FishSpinDays.Common.Admin.BindingModels
{
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Validation;
    using System.ComponentModel.DataAnnotations;

    public class PublicationBindingModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Title { get; set; }

        [MinLength(WebConstants.ContentMinLength, ErrorMessage = ValidationConstants.DescriptionNameMessage)]
        [Required(ErrorMessage = ValidationConstants.DescriptionNullMessage)]
        [SafeHtml(MaxLength = 50000, ErrorMessage = "Description contains invalid or dangerous content.")]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string Section { get; set; }
    }
}
