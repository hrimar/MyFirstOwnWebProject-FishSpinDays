using FishSpinDays.Services.Base;
using FishSpinDays.Services.Base.Interfaces;
using FishSpinDays.Tests.Mocks;
using FishSpinDays.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace FishSpinDays.Tests.Controllers.Home.HomeControllerTests
{
    [TestClass]
    public class TestIndex
    {
        [TestMethod]
        public void Index_ReturnsTheProperView()
        {
            //1. Arrange:           
            var mockService = new Mock<IBasePublicationsService>();
            var controller = new HomeController(mockService.Object);

            //2.Act:
            var result = controller.Index(null);

            //3.Assert:
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
