using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FishSpinDays.Data;
using FishSpinDays.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FishSpinDays.Common.Identity.ViewModels;

namespace FishSpinDays.Web.Areas.Identity.Pages.Users
{
    public class DetailsModel : BaseModel
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public DetailsModel(FishSpinDaysDbContext dbContex, UserManager<User> userManager, IMapper mapper)
            : base(dbContex)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public UserDetailsViewModel UserModel { get; set; }

        public IActionResult OnGet(string id) // Details
        {
            var currentUser = GetCurrentUser();

            var user = this.DbContext.Users
                .Include(u => u.Publications)
                .ThenInclude(u=>u.Comments)
                .FirstOrDefault(u => u.Id == id);

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