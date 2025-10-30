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
    using System.Threading.Tasks;

    [Authorize]
    public class AddModel : BaseModel
    {
        private readonly UserManager<User> userManager;

        public AddModel(UserManager<User> userManager, IIdentityService identityService) : base(identityService)
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
        [SafeHtml(MaxLength = 50000, ErrorMessage = "Description contains invalid or dangerous content.")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public User Author { get; set; }

        public async Task OnGetAsync()
        {
            this.Sections = await this.IdentityService.GetAllSectionsAsync().ConfigureAwait(false);
        }

        public async Task<IActionResult> OnPostCreatePublicationAsync()
        {
            if (!ModelState.IsValid)
            {
                // reload sections if validation fails
                this.Sections = await this.IdentityService.GetAllSectionsAsync().ConfigureAwait(false);
                return this.Page();
            }

            User author = await this.userManager.GetUserAsync(this.User).ConfigureAwait(false);
            var section = await this.IdentityService.GetSectionAsync(this.SectionId).ConfigureAwait(false);

            if (author == null || section == null)
            {
                return NotFound();
            }

            Publication publication = null;
            if (author.LockoutEnd < DateTime.Now || author.LockoutEnd == null)
            {
                publication = await this.IdentityService.CreatePublicationAsync(author, section, this.Title, this.Description).ConfigureAwait(false);

                if (publication == null)
                {
                    this.TempData.Put("__Message", new MessageModel()
                    {
                        Type = MessageType.Danger,
                        Message = WebConstants.UnsuccessfullOperation
                    });

                    // reload sections for retry
                    this.Sections = await this.IdentityService.GetAllSectionsAsync().ConfigureAwait(false);
                    return Page();
                }
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