namespace FishSpinDays.Common.Identity.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserBindingModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
