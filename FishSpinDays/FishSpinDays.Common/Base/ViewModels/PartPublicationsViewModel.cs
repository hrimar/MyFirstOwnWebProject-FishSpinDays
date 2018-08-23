using FishSpinDays.Common.Identity.ViewModels;
using System.Collections.Generic;


namespace FishSpinDays.Common.Base.ViewModels
{
   public class PartPublicationsViewModel
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public IEnumerable<PublicationShortViewModel> Publications { get; set; }
    }
}
