namespace FishSpinDays.Web.Areas.Identity.Pages.Chat
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}