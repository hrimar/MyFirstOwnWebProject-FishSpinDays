using FishSpinDays.Common.Base.ViewModels;
using FishSpinDays.Models;
using FishSpinDays.Services.Base;
using FishSpinDays.Services.Base.Interfaces;
using FishSpinDays.Tests.Mocks;
using FishSpinDays.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishSpinDays.Tests.Controllers.Home.PublicationsControllerTests
{
    [TestClass]
    public class DetailsTests
    {
        [TestMethod]
        [DataRow(1)]
        public void Details_FoundedModel_ReturnsTheProperView(int value)
        {
            //1. Arrange:        
            var publicationModel = new PublicationViewModel()
                {
                Id = 1,
                    AuthorId = "111",
                    Title = "On the river with Tisho",
                    Description = "I was on the river with my friend Pesho and catched so many fishes with only one metal lure",
                };

            var mockService = new Mock<IBasePublicationsService>();
            mockService
                .Setup(service => service.GetPublication(value))
                .Returns(publicationModel);
            var controller = new PublicationsController(mockService.Object);

            //2.Act:
            var result = controller.Details(value);

            //3.Assert:
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsNotNull((result as ViewResult).Model);
        }

        [TestMethod]
        [DataRow(1)]
        public void Details_FoundedModel_ReturnsNotNull(int value)
        {
            //1. Arrange:        
            var publicationModel = new PublicationViewModel()
            {
                Id = 1,
                AuthorId = "111",
                Title = "On the river with Tisho",
                Description = "I was on the river with my friend Pesho and catched so many fishes with only one metal lure",
            };

            var mockService = new Mock<IBasePublicationsService>();
            mockService
                .Setup(service => service.GetPublication(value))
                .Returns(publicationModel);
            var controller = new PublicationsController(mockService.Object);

            //2.Act:
            var result = controller.Details(value);

            //3.Assert:           
            Assert.IsNotNull((result as ViewResult).Model);
        }

        [TestMethod]
        [DataRow(1)]
        public void Details_WithValidCourse_ShoudCallService(int value)
        {
            //1. Arrange:           
            bool serviceCalled = false;

            var publicationModel = new PublicationViewModel()
            {
                Id = 1,
                AuthorId = "111",
                Title = "On the river with Tisho",
                Description = "I was on the river with my friend Pesho and catched so many fishes with only one metal lure",
            };

            var mockService = new Mock<IBasePublicationsService>();
            mockService
                .Setup(service => service.GetPublication(value))
                .Returns(publicationModel)
                .Callback(() => serviceCalled = true);

            var controller = new PublicationsController(mockService.Object);

            //2.Act:
            var result = controller.Details(1);

            //3.Assert:
            Assert.IsTrue(serviceCalled);
        }

       
    }
}
