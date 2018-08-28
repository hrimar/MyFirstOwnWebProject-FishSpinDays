namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using System;
    using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Web.Helpers;
    using FishSpinDays.Web.Helpers.Messages;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : AdminController
    {
        private readonly UserManager<User> userManager;
        private readonly IAdminUsersService userService;

        public UsersController(UserManager<User> userManager, IAdminUsersService userService)
        {    
            this.userManager = userManager;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            User currentUser = GetCurrentUser();
            
            var usersModel = this.userService.GetUsersWithourCurrentUser(currentUser.Id);
            if (usersModel == null)
            {
                return NotFound();
            }

            return View(usersModel);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            var currentUser = GetCurrentUser();

            if (id == currentUser.Id)
            {
                return Unauthorized();
            }

            var userModel = this.userService.GetUserModelById(id);
            if (userModel == null)
            {
                return NotFound();
            }

            return View(userModel);
        }

        [HttpGet]
        public IActionResult Ban(string id)
        {
            var currentUser = GetCurrentUser();
            if (id == currentUser.Id)
            {
                return Unauthorized();
            }

            var user = this.userService.GetUserById(id);
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

            var user = this.userService.GetUserById(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            bool isBaned = this.userService.IsUserBanned(user, model.LockoutEnd);

            if (!isBaned)
            {
                this.TempData.Put("__Message", new MessageModel()
                {
                    Type = MessageType.Warning,
                    Message = WebConstants.UnsuccesfullBan
                }); 
            }

            this.TempData.Put("__Message", new MessageModel()
            {
                Type = MessageType.Success,
                Message = WebConstants.SuccessfulBan
            });

            return RedirectToAction("/");
        }


        private User GetCurrentUser()
        {
            return this.userManager.GetUserAsync(this.User).Result;
        }
    }
}