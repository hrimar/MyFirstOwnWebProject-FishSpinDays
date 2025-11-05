namespace FishSpinDays.Common.API.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for updating user profile information
    /// </summary>
    public class UpdateUserModel
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}