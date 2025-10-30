namespace FishSpinDays.Common.API.Models.Comments
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for adding comments to publications
    /// </summary>
    public class AddCommentModel
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Publication ID must be a valid positive number.")]
        public int PublicationId { get; set; }

        [Required(ErrorMessage = "Comment text is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 1000 characters.")]
        public string Text { get; set; }
    }
}