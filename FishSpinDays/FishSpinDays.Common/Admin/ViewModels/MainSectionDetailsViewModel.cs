namespace FishSpinDays.Common.Admin.ViewModels
{
    using FishSpinDays.Common.Identity.ViewModels;
    using System.Collections.Generic;

    public class MainSectionDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<SectionShortViewModel> Sections { get; set; }
    }
}
