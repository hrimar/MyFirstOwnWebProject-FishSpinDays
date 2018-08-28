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

            //3. Assert:           
            Assert.IsNotNull((result as ViewResult).Model);
        }

        [TestMethod]
        [DataRow(1)]
        public void Details_WithValidPublication_ShoudCallService(int value)
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

        [TestMethod]
        public void Details_RetursCorrectModelInformation()
        {
            // 1.
            var service = new Mock<IBasePublicationsService>();
            var controller = new PublicationsController(service.Object);

            var publicationModel = new PublicationViewModel()
            {
                Id = 1,
                AuthorId = "111",
                Title = "On the river with Tisho",
                Description = "I was on the river with my friend Pesho and catched so many fishes with only one metal lure",
            };

            service.Setup(serv => serv.GetPublication(publicationModel.Id))
                .Returns(new PublicationViewModel()
                {
                    Id = publicationModel.Id,
                    AuthorId = publicationModel.AuthorId,
                    Title = publicationModel.Title,
                    Description = publicationModel.Description
                });

            // 2. Act:
            var result = controller.Details(publicationModel.Id);

            //3. Assert:    
            var resultModel = result as ViewResult;
            Assert.IsNotNull(resultModel.Model);
            Assert.IsInstanceOfType(resultModel.Model, typeof(PublicationViewModel));

            var productType = (resultModel.Model as PublicationViewModel);
            Assert.AreEqual(publicationModel.Id, productType.Id);
            Assert.AreEqual(publicationModel.AuthorId, productType.AuthorId);
            Assert.AreEqual(publicationModel.Title, productType.Title);
            Assert.AreEqual(publicationModel.Description, productType.Description);
        }

    }
}
