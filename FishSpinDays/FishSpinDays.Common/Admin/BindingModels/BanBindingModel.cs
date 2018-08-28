namespace FishSpinDays.Common.Admin.BindingModels
{
    using FishSpinDays.Common.Validation;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BanBindingModel
    {
        public string Id { get; set; }

        [Display(Name = ValidationConstants.BlockEnd)]      
        public DateTime LockoutEnd { get; set; }
    }
}
