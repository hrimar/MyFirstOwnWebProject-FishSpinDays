using FishSpinDays.Common.Admin.BindingModels;
using FishSpinDays.Data;
using FishSpinDays.Services.Admin;
using FishSpinDays.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishSpinDays.Tests.Services.IdentityPublications
{
    [TestClass]
   public class AddPublicationTests
    {
        private AdminSectionsService service;
        private FishSpinDaysDbContext dbContext;

        [TestInitialize]
        public void InitilizeTests()
        {
            this.dbContext = MockDbContext.GetContext();
            var mapper = MockAutomapper.GetMapper();
            this.service = new AdminSectionsService(dbContext, mapper);
        }


        [TestMethod]
        public void AddMainSection_WithPropeMainSection_ShouldAddCorrectly()
        {
            // 1. Arrange
            var mainSectionName = "Test trips";

            var mainSectionModel = new CreateMainSectionBindingModel()
            {
                Name = mainSectionName,
            };

            // 2. Act
            this.service.AddMainSection(mainSectionModel);

            // 3. Asserts 
            Assert.AreEqual(1, this.dbContext.MainSections.Count());
            var mainSection = this.dbContext.MainSections.First();
            Assert.AreEqual(mainSectionName, mainSection.Name);
        }
    }
}
