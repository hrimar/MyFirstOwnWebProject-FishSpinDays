namespace FishSpinDays.Web.Areas.Identity.Pages.Publications
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Validation;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize]
    public class AddModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public AddModel(UserManager<User> userManager, IIdentityService identityService)
      : base(identityService)
        {
            this.userManager = userManager;

            this.Sections = new List<SelectListItem>();
        }

        [BindProperty]
        [Required(ErrorMessage = ValidationConstants.RequireSectionSpecified)]
        [Display(Name = ValidationConstants.SectionDisplayName)]
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
            this.Sections = this.IdentityService.GettAllSections();
        }

        public IActionResult OnPostCreatePublication()
        {
            if (!ModelState.IsValid)
            {
                return this.Page();
            }

            User author = this.userManager.GetUserAsync(this.User).Result;
            var section = this.IdentityService.GetSection(this.SectionId);

            if (author == null || section == null)
            {
                return NotFound();
            }

            var publication = this.IdentityService.CreatePublication(author, section, this.Title, this.Description);

            if (publication == null)
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
                Message = WebConstants.SuccessfullPublication
            });

            return Redirect($"/Publications/Details/{publication.Id}");
        }
    }
}