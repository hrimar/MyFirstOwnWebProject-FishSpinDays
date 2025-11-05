namespace FishSpinDays.Common.API.Models.Publications
{
    using System;

    public class PublicationResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int Likes { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public string Section { get; set; }
        public int CommentsCount { get; set; }
    }
}