using FishSpinDays.Services.Base.Interfaces;
using FishSpinDays.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishSpinDays.Common.Base.ViewModels;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FishSpinDays.Tests.Controllers.Home.HomeControllerTests
{
    [TestClass]
    public class TestIndex
    {
        [TestMethod]
        public async Task Index_ReturnsTheProperView()
        {
            //1. Arrange:           
            var mockService = new Mock<IBasePublicationsService>();
            mockService
                .Setup(service => service.GetAllPublicationsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PublicationShortViewModel>());
            mockService
                .Setup(service => service.TotalPublicationsCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockService.Object, mockLogger.Object);

            //2.Act:
            var result = await controller.Index(null);

            //3.Assert:
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}
