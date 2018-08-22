namespace FishSpinDays.Common.Admin.BindingModels
{
    using FishSpinDays.Common.Constants;
    using System.ComponentModel.DataAnnotations;

    public class CreateMainSectionBindingModel
    {
        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Name { get; set; }
    }
}
