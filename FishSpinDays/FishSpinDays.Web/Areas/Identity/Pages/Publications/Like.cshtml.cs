namespace FishSpinDays.Web.Areas.Identity.Pages.Publications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Identity;
    using FishSpinDays.Data;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Models;

    [Authorize]
    public class LikeModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public LikeModel(FishSpinDaysDbContext dbContex, UserManager<User> userManager)
            : base(dbContex)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet(int id)
        {      
            var publication = this.DbContext.Publications.FirstOrDefault(p => p.Id == id);
            if (publication == null)
            {
                return NotFound();
            }

            var publicationAuthorId = publication.AuthorId;
            var currentUser = this.userManager.GetUserAsync(this.User).Result;
            if (publicationAuthorId == currentUser.Id)
            {
                return Unauthorized();
            }

            publication.Likes++;
            this.DbContext.SaveChanges();

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Info,
                Message = "Thank you for your vote."
            });

            return Redirect($"/Publications/Details/{id}");
        }


    }
}