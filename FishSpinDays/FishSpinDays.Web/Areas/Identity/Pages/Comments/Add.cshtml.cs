using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FishSpinDays.Common.Constants;
using FishSpinDays.Data;
using FishSpinDays.Models;
using FishSpinDays.Web.Helpers;
using FishSpinDays.Web.Helpers.Messages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FishSpinDays.Web.Areas.Identity.Pages.Comments
{
    [Authorize]
    public class AddModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public AddModel(FishSpinDaysDbContext dbContext, UserManager<User> userManager)
      : base(dbContext)
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
        {
        }

        // [ValidateAntiForgeryToken]
        public IActionResult OnPostCreateComment(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Page();
            }

            User author = this.userManager.GetUserAsync(this.User).Result;
            var publcation = this.DbContext.Publications.Find(id);

            this.Author = author.UserName;

            Comment comment = new Comment() 
            {
                Publication = publcation,
                Text = this.Text,
                Author = author
            };

            try
            {
                this.DbContext.Comments.Add(comment);
                this.DbContext.SaveChanges();

                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Success,
                    Message = "Thank you for your comment."
                });

                return Redirect($"/Publications/Details/{id}");
            }
            catch (Exception)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Danger,
                    Message = "Unsuccessfull operation!"
                });

                return Page();
            }
        }
    }
}