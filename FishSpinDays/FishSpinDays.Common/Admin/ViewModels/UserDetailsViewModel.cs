namespace FishSpinDays.Common.Admin.ViewModels
{
    using System.Collections.Generic;

    public class UserDetailsViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Roles { get; set; } // 
    }
}
