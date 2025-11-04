namespace FishSpinDays.Common.API.Models.Comments
{
    using System;

    /// <summary>
    /// DTO model for comment response
    /// </summary>
    public class CommentResponseModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public string Author { get; set; }
        public int Likes { get; set; }
        public int UnLikes { get; set; }
        public int PublicationId { get; set; }
        public string PublicationTitle { get; set; }
    }
}