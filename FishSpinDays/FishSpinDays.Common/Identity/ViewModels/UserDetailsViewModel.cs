namespace FishSpinDays.Common.Identity.ViewModels
{
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Identity.ViewModels;
    using System.Collections.Generic;

    public class UserDetailsViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }
       
        public string Email { get; set; }

        public ICollection<PublicationViewModel> Publications { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }
    }
}
