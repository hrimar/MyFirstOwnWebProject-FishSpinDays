namespace FishSpinDays.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;

    public class User : IdentityUser
    {
        public User()
        {
            this.Publications = new List<Publication>();
            this.Comments = new List<Comment>();
        }
                
        public ICollection<Publication> Publications { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
