namespace FishSpinDays.Services.Admin
{
    using AutoMapper;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Admin.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AdminUsersService : BaseService, IAdminUsersService
    {
        public AdminUsersService(FishSpinDaysDbContext dbContex, IMapper mapper) 
            : base(dbContex, mapper)
        {  }

        public IEnumerable<UserShortViewModel> GetUsersWithourCurrentUser(string currentUserId)
        {
            var users = this.DbContext.Users
                .Where(u => u.Id != currentUserId)
                .ToList();

            if (users == null)
            {
                return null;
            }

            var model = this.Mapper.Map<IEnumerable<UserShortViewModel>>(users);

            return model;
        }

        public UserDetailsViewModel GetUserModelById(string id)
        {
            var user = this.DbContext.Users
                .Include(u => u.Publications)
                .ThenInclude(u => u.Comments)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            var model = this.Mapper.Map<UserDetailsViewModel>(user);

            return model;
        }

        public User GetUserById(string id)
        {
            var user = this.DbContext.Users
                .Include(u => u.Publications)
                .ThenInclude(u => u.Comments)
                .FirstOrDefault(u => u.Id == id);
                           
            return user;
        }

        public bool IsUserBanned(User user, DateTime lockoutEnd)
        {
            try
            {
                user.LockoutEnd = lockoutEnd;
                this.DbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
