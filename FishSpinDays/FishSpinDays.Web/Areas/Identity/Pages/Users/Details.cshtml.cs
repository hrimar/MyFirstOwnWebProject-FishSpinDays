namespace FishSpinDays.Web.Areas.Identity.Pages.Users
{
    using AutoMapper;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Common.Identity.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using FishSpinDays.Services.Identity.Interfaces;
    using System.Threading.Tasks;

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

        public async Task<IActionResult> OnGetAsync(string id) 
        {
            var currentUser = await GetCurrentUserAsync().ConfigureAwait(false);

            var user = await this.IdentityService.GetUserByIdAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            var model = this.mapper.Map<UserDetailsViewModel>(user);
            this.UserModel = model;

            return Page();
        }

        private async Task<User> GetCurrentUserAsync()
        {
            return await this.userManager.GetUserAsync(this.User).ConfigureAwait(false);
        }
    }
}