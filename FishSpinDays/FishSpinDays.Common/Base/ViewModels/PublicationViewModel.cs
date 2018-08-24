namespace FishSpinDays.Common.Base.ViewModels
{
    using FishSpinDays.Common.Identity.ViewModels;
    using System;
    using System.Collections.Generic;
   
    public class PublicationViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public int Likes { get; set; }

        public string Author { get; set; }
        public string AuthorId { get; set; }

        public string Section { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
