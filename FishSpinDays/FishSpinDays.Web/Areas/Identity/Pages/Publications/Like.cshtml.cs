namespace FishSpinDays.Web.Areas.Identity.Pages.Publications
{    
    using Microsoft.AspNetCore.Identity;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Identity.Interfaces;

    [Authorize]
    public class LikeModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public LikeModel(UserManager<User> userManager, IIdentityService identityService)
            : base(identityService)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet(int id)
        {            
            var publication = this.IdentityService.GetPublicationById(id);
            if (publication == null)
            {
                return NotFound();
            }

            var publicationAuthorId = publication.AuthorId;
            var currentUser = this.userManager.GetUserAsync(this.User).Result;
            if (publicationAuthorId == currentUser.Id)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Warning,
                    Message = "You can not like yours publications."
                });
               
                return Redirect($"/Publications/Details/{id}");
            }
                       
            bool isLikedPublication = this.IdentityService.IsLikedPublication(publication);

            if (!isLikedPublication)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Warning,
                    Message = "Unsuccessful vote."
                }); 
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Info,
                Message = "Thank you for your vote."
            });

            return Redirect($"/Publications/Details/{id}");
        }
    }
}