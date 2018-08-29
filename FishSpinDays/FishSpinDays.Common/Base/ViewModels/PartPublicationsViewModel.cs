namespace FishSpinDays.Common.Base.ViewModels
{
    using FishSpinDays.Common.Identity.ViewModels;
    using System.Collections.Generic;

    public class PartPublicationsViewModel
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public IEnumerable<PublicationShortViewModel> Publications { get; set; }
    }
}
