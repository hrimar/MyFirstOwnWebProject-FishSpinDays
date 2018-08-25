using FishSpinDays.Common.Admin.ViewModels;
using FishSpinDays.Common.Constants;
using FishSpinDays.Models;
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

namespace FishSpinDays.Tests.Controllers.Admin.UsersControllerTests
{
    [TestClass]
    public class IndexTests
    {
        [TestMethod]
        public void Index_ShoudBeAccesseibleByAdmin()
        {
            var controller = new UsersController(null, null, null);
            //controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal();


            // подаване на потребител:
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
        public void Index_ShoudReturnAllUsersExceptCurrent() // !!!
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

            //var mockUserManager = new Mock<UserManager<User>>();

            var mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(um => um.GetUserAsync(null))
                .ReturnsAsync(users[1]);

            var controller = new UsersController(mockDbContext,
                 MockAutomapper.GetMapper(), mockUserManager.Object);

            //2. act:
            var result = controller.Index() as ViewResult;

            //. assert:
            Assert.IsNotNull(result);
            var model = result.Model as IEnumerable<UserShortViewModel>;
            CollectionAssert.AreEqual(new[] { "111", "333", "444" },
                                            model.Select(u => u.Id).ToArray());
        }
    }
}
