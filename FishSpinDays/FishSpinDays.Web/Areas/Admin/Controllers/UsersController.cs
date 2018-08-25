namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class UsersController : AdminController
    {
        private readonly FishSpinDaysDbContext contex;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public UsersController(FishSpinDaysDbContext contex, IMapper mapper,
            UserManager<User> userManager)
        {
            this.contex = contex;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            User currentUser = GetCurrentUser();

            var users = this.contex.Users
                .Where(u => u.Id != currentUser.Id) 
                .ToList();

            var model = this.mapper.Map<IEnumerable<UserShortViewModel>>(users);

            return View(model);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var currentUser = GetCurrentUser();

            if (id == currentUser.Id)
            {
                return Unauthorized();
            }

            var user = this.contex.Users
                .Include(u=>u.Publications)
                .FirstOrDefault(u=>u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
                        
            var model = this.mapper.Map<UserDetailsViewModel>(user);

            return View(model);
        }

        [HttpGet]
        public IActionResult Ban(string id)
        {
            var currentUser = GetCurrentUser();
            if (id == currentUser.Id)
            {
                return Unauthorized();
            }

            var user = this.contex.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

             ViewData["Message"] = user.UserName;

            var model = new BanBindingModel()
            {
                Id = id,
                LockoutEnd = DateTime.Now.Date
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Ban(BanBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = this.contex.Users.Find(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.LockoutEnd = model.LockoutEnd;
            this.contex.SaveChanges();

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = "You have succesfully put a BAN to this user."
            });
                          
            return RedirectToAction("/");
        }


        private User GetCurrentUser()
        {
            return this.userManager.GetUserAsync(this.User).Result;
        }
    }
}