namespace FishSpinDays.Models
{
    using FishSpinDays.Common.Constants;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Section
    {
        public Section()
        {
            this.Publications = new List<Publication>();
        }

        public int Id { get; set; }

        public ICollection<Publication> Publications { get; set; }

        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Name { get; set; }


    }
}
