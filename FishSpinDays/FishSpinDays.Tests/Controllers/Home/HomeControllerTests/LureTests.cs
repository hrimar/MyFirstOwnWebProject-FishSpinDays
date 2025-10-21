using FishSpinDays.Common.Base.ViewModels;
using FishSpinDays.Common.Constants;
using FishSpinDays.Services.Base;
using FishSpinDays.Services.Base.Interfaces;
using FishSpinDays.Tests.Mocks;
using FishSpinDays.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace FishSpinDays.Tests.Controllers.Home.HomeControllerTests
{
    [TestClass]
    public class LureTests
    {
        [TestMethod]
        public async Task Lures_ReturnsTheProperViews()
        {
            //1. Arrange:           
            var mockService = new Mock<IBasePublicationsService>();
            mockService
                .Setup(service => service.GetAllPublicationsInThisSectionAsync(WebConstants.HandLures, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] {new PublicationShortViewModel()
                {
                    Id = 4,
                    Title = "The greates lures",
                    Description = "I was on the river with my friend Pesho and catched so many fishes with only one metal lure",
                } });
            var controller = new HomeController(mockService.Object);

            //2.Act:
            var result = await controller.Lures();

            //3.Assert:
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsNotNull((result as ViewResult).Model);
        }
               
    }
}
