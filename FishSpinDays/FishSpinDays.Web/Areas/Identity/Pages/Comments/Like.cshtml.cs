namespace FishSpinDays.Web.Areas.Identity.Pages.Comments
{
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Common.Constants;

    [Authorize]
    public class LikeModel : BaseModel
    {
        public LikeModel(IIdentityService identityService)
            : base(identityService)
        {   }

        public IActionResult OnGet(int id)
        {
            var comment = this.IdentityService.GetCommentById(id);

            bool isCommentLiked = this.IdentityService.IsLikedComment(comment);

            if (!isCommentLiked)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Warning,
                    Message = WebConstants.UnsuccesfullVoting
                }); 
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Info,
                Message = WebConstants.ThankYouForYourVoute
            });

            return Redirect("/");
        }
    }
}