namespace FishSpinDays.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Validation;

    public class Publication
    {
        public Publication()
        {
            this.CreationDate = DateTime.Now;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Title { get; set; }


        [MinLength(WebConstants.ContentMinLength, ErrorMessage = ValidationConstants.DescriptionNameMessage)]
        [Required(ErrorMessage = ValidationConstants.DescriptionNullMessage)] 
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [Range(0, int.MaxValue)]
        public int Likes { get; set; }

        [Required]
        public string AuthorId { get; set; }
        public User Author { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
