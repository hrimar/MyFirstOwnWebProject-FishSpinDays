namespace FishSpinDays.Common.API.Models.Users
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// DTO model for current user response
    /// </summary>
    public class CurrentUserResponseModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool IsLockedOut { get; set; }
    }
}