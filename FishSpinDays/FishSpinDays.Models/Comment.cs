using FishSpinDays.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace FishSpinDays.Models
{
   public class Comment
    {
        public int Id { get; set; }

        [MinLength(WebConstants.NameMinLength)]
        public string Text { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}
