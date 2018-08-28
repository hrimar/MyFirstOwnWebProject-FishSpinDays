using FishSpinDays.Common.Admin.ViewModels;
using FishSpinDays.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishSpinDays.Services.Admin.Interfaces
{
   public interface IAdminUsersService
    {
        IEnumerable<UserShortViewModel> GetUsersWithourCurrentUser(string currentUserId);

        UserDetailsViewModel GetUserModelById(string id);

        User GetUserById(string id);

        bool IsUserBanned(User user, DateTime lockoutEnd);
    }
}
