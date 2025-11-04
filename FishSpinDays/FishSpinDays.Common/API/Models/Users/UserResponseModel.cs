namespace FishSpinDays.Common.API.Models.Users
{
    using System;

    /// <summary>
    /// DTO model for user response
    /// </summary>
    public class UserResponseModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public int PublicationsCount { get; set; }
        public int CommentsCount { get; set; }
    }
}