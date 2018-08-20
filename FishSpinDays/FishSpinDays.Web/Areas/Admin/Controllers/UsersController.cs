﻿namespace FishSpinDays.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UsersController : AdminController
    {
        private readonly FishSpinDaysDbContext contex;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public UsersController(FishSpinDaysDbContext conte, IMapper mapper,
            UserManager<User> userManager)
        {
            this.contex = conte;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            User currentUser = GetCurrentUser();

            var users = this.contex.Users
                .Where(u => u.Id != currentUser.Id) // ToString - test this!
                .ToList();

            var model = this.mapper.Map<IEnumerable<UserShortViewModel>>(users);

            return View(model);
        }
       

        //public  IActionResult Details(string id)
        //{
        //    var currentUser = GetCurrentUser();

        //    if (id == currentUser.Id)
        //    {
        //        return Unauthorized();
        //    }
            
        //    var user =  this.contex.Users.Find(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var roles =  this.userManager.GetRolesAsync(user).Result; // remove this!

        //    var model = this.mapper.Map<UserDetailsViewModel>(user);

        //    model.Roles = roles; //  TODO: Change this with his Publications.Names !!!

        //    return View(model);
        //}

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

           // user.LockoutEnd= 
           // TODO: make page for set the date for ban!

            return View();
        }


        private User GetCurrentUser()
        {
            return this.userManager.GetUserAsync(this.User).Result;
        }
    }
}