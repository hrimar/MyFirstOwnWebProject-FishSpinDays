namespace FishSpinDays.Common.Base.ViewModels
{
    using System;

    public class PublicationShortViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CoverImage { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }              

        public string Author { get; set; }       
    }
}
