namespace FishSpinDays.Web.Areas.Identity.Pages
{
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class BaseModel : PageModel
    {
        public BaseModel(IIdentityService identityService)
        {
            this.IdentityService = identityService;
        }
               
        public IIdentityService IdentityService { get; private set; }
    }
}