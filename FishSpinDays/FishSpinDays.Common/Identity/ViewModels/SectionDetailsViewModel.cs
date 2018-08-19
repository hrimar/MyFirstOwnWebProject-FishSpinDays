﻿namespace FishSpinDays.Common.Identity.ViewModels
{
    using FishSpinDays.Common.Identity.ViewModels;
    using System.Collections.Generic;

    public class SectionDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<PublicationViewModel> Publications { get; set; }
    }
}
