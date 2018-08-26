using FishSpinDays.Common.Admin.ViewModels;
using FishSpinDays.Models;
using FishSpinDays.Tests.Mocks;
using FishSpinDays.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishSpinDays.Tests.Controllers.Admin.UsersControllerTests
{
    [TestClass]
    public class DetailsTests
    {
        private const string  inputUserId = "111";

        //[TestMethod]
        //public void Details_UserByID_ShoudReturnsShoundedUser(string inputUserId)
        //{
        //    var user = new User
        //    {
        //        Id = "111"
        //    };

        //    var mockDbContext = MockDbContext.GetContext();
        //    mockDbContext.Users.Add(user);
        //    mockDbContext.SaveChanges();

        //    var mockUserStore = new Mock<IUserStore<User>>();

        //    var mockUserManager = new Mock<UserManager<User>>(
        //        new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        //    mockUserManager.Setup(um => um.GetUserAsync(null))
        //        .ReturnsAsync(user);

        //    var controller = new UsersController(mockDbContext,
        //         MockAutomapper.GetMapper(), mockUserManager.Object);

        //    //2. act:
        //    var result = controller.Details(user.Id) as ViewResult;

        //    //. assert:
        //    Assert.IsNotNull(result);
        //    var model = result.Model as UserDetailsViewModel;
        //    Assert.AreEqual(new User { Id="111" }, model.Id);
        //}

        //[TestMethod]
        //public void Details_NullUser_ShoudReturnsNotFound(string inputUserId)
        //{
        //    User user = null;

        //    var mockDbContext = MockDbContext.GetContext();
        //    mockDbContext.Users.Add(user);
        //    mockDbContext.SaveChanges();

        //    var mockUserStore = new Mock<IUserStore<User>>();

        //    var mockUserManager = new Mock<UserManager<User>>(
        //        new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
        //    mockUserManager.Setup(um => um.GetUserAsync(null))
        //        .ReturnsAsync(user);

        //    var controller = new UsersController(mockDbContext,
        //         MockAutomapper.GetMapper(), mockUserManager.Object);

        //    //2. act:
        //    //var result = controller.Details(user.Id) as ViewResult;
        //    ////. assert:
        //    //Assert.IsNotNull(result);
        //    //var model = result.Model as UserDetailsViewModel;
        //    //Assert.AreEqual(new User { Id = "111" }, model.Id);


        //    Action addMainSection = () => controller.Details(user.Id);
        //    // 3. Asserts 
        //    Assert.ThrowsException<ArgumentNullException>(addMainSection);
        //}
    }
}
