namespace FishSpinDays.Models
{
    using FishSpinDays.Common.Constants;
    using System;
    using System.ComponentModel.DataAnnotations;
    
    public class Comment
    {
        public Comment()
        {
            this.CreationDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [MinLength(WebConstants.NameMinLength)]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Range(0, int.MaxValue)]
        public int Likes { get; set; }

        [Range(0, int.MaxValue)]
        public int UnLikes { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string PublicationId { get; set; }
        public Publication Publication { get; set; }
    }
}
