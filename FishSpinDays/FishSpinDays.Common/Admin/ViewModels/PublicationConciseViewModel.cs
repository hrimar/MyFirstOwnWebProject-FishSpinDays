namespace FishSpinDays.Common.Admin.ViewModels
{
    using System;
    using System.Collections.Generic;
   
    public class PublicationConciseViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
      
        public DateTime CreationDate { get; set; }

        public int Likes { get; set; }

        public string Author { get; set; }

        public string Section { get; set; }
       
    }
}
