namespace FishSpinDays.Web.Areas.Identity.Pages.Users
{
    using AutoMapper;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Common.Identity.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Services.Identity.Interfaces;

    [Authorize]
    public class DetailsModel : BaseModel
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public DetailsModel(UserManager<User> userManager, IMapper mapper, IIdentityService identityService)
            : base(identityService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public UserDetailsViewModel UserModel { get; set; }

        public IActionResult OnGet(string id) 
        {
            var currentUser = GetCurrentUser();

            var user = this.IdentityService.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            var model = this.mapper.Map<UserDetailsViewModel>(user);
            this.UserModel = model;

            return Page();
        }

        private User GetCurrentUser()
        {
            return this.userManager.GetUserAsync(this.User).Result;
        }
    }
}