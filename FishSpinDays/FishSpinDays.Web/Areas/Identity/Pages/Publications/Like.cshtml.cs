using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FishSpinDays.Data;
using FishSpinDays.Web.Helpers;
using FishSpinDays.Web.Helpers.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FishSpinDays.Web.Areas.Identity.Pages.Publications
{
    public class LikeModel : BaseModel
    {
        public LikeModel(FishSpinDaysDbContext dbContex) 
            : base(dbContex)
        {  }

        public IActionResult OnGetLike(int id)
        {
            var publication = this.DbContext.Publications.FirstOrDefault(p=>p.Id == id);
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