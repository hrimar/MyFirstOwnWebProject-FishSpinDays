using FishSpinDays.Common.Admin.ViewModels;
using FishSpinDays.Common.Constants;
using FishSpinDays.Models;
using FishSpinDays.Services.Admin.Interfaces;
using FishSpinDays.Tests.Mocks;
using FishSpinDays.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FishSpinDays.Tests.Controllers.Admin.UsersControllerTests
{
    [TestClass]
    public class IndexTests
    {
        [TestMethod]
        public void Index_ShoudBeAccesseibleByAdmin()
        {
            var controller = new UsersController(null, null);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Role, WebConstants.AdminRole)
                    }))
                }
            };

            Assert.IsTrue(controller.User.IsInRole(WebConstants.AdminRole));
        }

        [TestMethod]
        public async Task Index_ShoudReturnNotNull()
        {
            var users = new[]
            {
                new User() { Id = "111" },
                new User() { Id = "222" },
            };

            var mockDbContext = MockDbContext.GetContext();
            mockDbContext.Users.AddRange(users);
            mockDbContext.SaveChanges();

            var mockUserStore = new Mock<IUserStore<User>>();

            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(users[1]);


            var mockService = new Mock<IAdminUsersService>();
            mockService
                .Setup(service => service.GetUsersWithourCurrentUser("222"))
                .Returns(new[] {new UserShortViewModel()
                {
                    Id = "111",
                    UserName = "Miisho",
                    Email = "mishi@abv.bg"
                }});

            var controller = new UsersController(mockUserManager.Object, mockService.Object);

            //2. act:
            var result = await controller.Index() as ViewResult;

            //3. assert:
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task Index_ShoudReturnAllUsersExceptCurrent()
        {
            var users = new[]
            {
                new User() { Id = "111" },
                new User() { Id = "222" },
                new User() { Id = "333" },
                new User() { Id = "444" },
            };

            var mockDbContext = MockDbContext.GetContext();
            mockDbContext.Users.AddRange(users);
            mockDbContext.SaveChanges();

            var mockUserStore = new Mock<IUserStore<User>>();

            var mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(users[1]);

            var mockService = new Mock<IAdminUsersService>();
            mockService
                .Setup(service => service.GetUsersWithourCurrentUser("222"))
                .Returns(new[]
                {
                    new UserShortViewModel() { Id = "111" },
                    new UserShortViewModel() { Id = "333" },
                    new UserShortViewModel() { Id = "444" },
                });

            var controller = new UsersController(mockUserManager.Object, mockService.Object);

            //2. act:
            var result = await controller.Index() as ViewResult;

            //3. assert:
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<UserShortViewModel>;
            CollectionAssert.AreEqual(new[] { "111", "333", "444" }, model.Select(u => u.Id).ToArray());
        }
    }
}
