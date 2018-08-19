namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using FishSpinDays.Common.Constants;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area(WebConstants.AdminArea)]
    [Authorize(Roles = WebConstants.AdminRole)]
    public abstract class AdminController : Controller
    {
    }
}