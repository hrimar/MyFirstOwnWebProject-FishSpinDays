namespace FishSpinDays.Common.API.Models.Publications
{
    using System;

    /// <summary>
    /// DTO model for user publication response
    /// </summary>
    public class UserPublicationResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public int Likes { get; set; }
        public string Section { get; set; }
        public int CommentsCount { get; set; }
    }
}