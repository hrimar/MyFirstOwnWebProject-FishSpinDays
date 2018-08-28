namespace FishSpinDays.Web.Areas.Identity.Pages.Publications
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Validation;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize]
    public class AddModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public AddModel(FishSpinDaysDbContext dbContext, UserManager<User> userManager)
      : base(dbContext)
        {           
            this.userManager = userManager;                        
            this.Sections = new List<SelectListItem>();
        }

       
        [BindProperty]
        [Required(ErrorMessage = "You have to specify a section.")]
        [Display(Name = "Section")]
        public int SectionId { get; set; }

        public IEnumerable<SelectListItem> Sections { get; set; }

        [BindProperty]
        [Required]
        [StringLength(WebConstants.NameMaxLength, MinimumLength = WebConstants.NameMinLength)]
        public string Title { get; set; }

        [BindProperty]
        [MinLength(WebConstants.ContentMinLength, ErrorMessage = ValidationConstants.DescriptionNameMessage)]
        [Required(ErrorMessage = ValidationConstants.DescriptionNullMessage)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public User Author { get; set; }

        public void OnGet()
        {
            this.Sections = this.DbContext.Sections
                .Select(b => new SelectListItem()
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                })
                .ToList();
        }
        
        public IActionResult OnPostCreatePublication()
        {
            if (!ModelState.IsValid)
            {
                return this.Page();
            }

            User author = this.userManager.GetUserAsync(this.User).Result;
            var sectionId = this.DbContext.Sections.Find(this.SectionId).Id;

            Publication publication = new Publication() 
            {
                SectionId = sectionId,
                Title = this.Title,
                Description = this.Description,
                Author = author
            };

            try
            {
                this.DbContext.Publications.Add(publication);
                this.DbContext.SaveChanges();

                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Success,
                    Message = "You have created succesfully a new publication."
                });

                return Redirect($"/Publications/Details/{publication.Id}");
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