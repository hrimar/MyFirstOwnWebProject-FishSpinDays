namespace FishSpinDays.Common.Admin.ViewModels
{
    using System.Collections.Generic;

    public class MainSectionDto
    {      
        public string Name { get; set; }

        public ICollection<string> Sections { get; set; }
    }
}
