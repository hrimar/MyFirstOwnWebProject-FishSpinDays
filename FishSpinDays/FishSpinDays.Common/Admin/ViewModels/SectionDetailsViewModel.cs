namespace FishSpinDays.Common.Admin.ViewModels
{
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Identity.ViewModels;
    using System.Collections.Generic;

    public class SectionDetailsViewModel
    {        
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<PublicationConciseViewModel> Publications { get; set; }
    }
}
