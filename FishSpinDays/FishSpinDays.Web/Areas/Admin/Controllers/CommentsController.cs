namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Mvc;

    public class CommentsController : AdminController
    {
        private readonly IAdminPublicationsService adminPublicationsService;
        
        public CommentsController(IAdminPublicationsService adminSectionService)
        {
            this.adminPublicationsService = adminSectionService;
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            bool isDeleted = this.adminPublicationsService.DeleteComment(id);
            if (!isDeleted)
            {
                return NotFound();
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = WebConstants.DeletedComment
            });

            return Redirect("/");
        }
        
    }
}