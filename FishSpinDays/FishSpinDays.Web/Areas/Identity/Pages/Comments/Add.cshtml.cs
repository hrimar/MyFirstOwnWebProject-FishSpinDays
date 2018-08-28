namespace FishSpinDays.Web.Areas.Identity.Pages.Comments
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Models;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Services.Identity.Interfaces;

    [Authorize]
    public class AddModel : BaseModel
    {
        private readonly UserManager<User> userManager;
       
        public AddModel(UserManager<User> userManager, IIdentityService identityService)
      : base(identityService)
        {
            this.userManager = userManager;
           
            this.CreationDate = DateTime.Now;
        }

        [BindProperty]
        [MinLength(WebConstants.NameMinLength)]
        public string Text { get; set; }

        public string Author { get; set; }

        public DateTime CreationDate { get; set; }

        public int Likes { get; set; }

        public int UnLikes { get; set; }

        public void OnGet()
        { }

        public IActionResult OnPostCreateComment(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Page();
            }

            User author = this.userManager.GetUserAsync(this.User).Result;
            this.Author = author.UserName;
                 
            var publication = this.IdentityService.GetPublicationById(id);
            if (publication == null)
            {
                return NotFound();
            }
            
            var comment = this.IdentityService.AddComment(author, publication, this.Text);

            if (comment == null)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Danger,
                    Message = WebConstants.UnsuccessfullOperation
                });

                return Page();
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = WebConstants.ThankForComment
            });

            return Redirect($"/Publications/Details/{id}");
        }
    }
}