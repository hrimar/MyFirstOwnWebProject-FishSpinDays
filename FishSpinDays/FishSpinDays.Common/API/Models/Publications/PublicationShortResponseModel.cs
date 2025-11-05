namespace FishSpinDays.Common.API.Models.Publications
{
    using System;

    public class PublicationShortResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
        public string Section { get; set; }
        public int CommentsCount { get; set; }
    }
}