using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Data;
using FishSpinDays.Web.Helpers;
using FishSpinDays.Web.Helpers.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FishSpinDays.Web.Areas.Identity.Pages.Comments
{    
    public class UnLikeModel : BaseModel
    {
        public UnLikeModel(FishSpinDaysDbContext dbContex)
            : base(dbContex)
        { }

        public IActionResult OnGet(int id)
        {
            var comment = this.DbContext.Comments.FirstOrDefault(p => p.Id == id);
            comment.UnLikes++;
            this.DbContext.SaveChanges();

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Info,
                Message = "Thank you for your vote about this comment."
            });

            return Redirect("/");
        }
    }
}