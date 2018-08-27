using FishSpinDays.Common.Admin.BindingModels;
using FishSpinDays.Common.Validation;
using FishSpinDays.Data;
using FishSpinDays.Services.Admin;
using FishSpinDays.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishSpinDays.Tests.Services.AdminSections
{
    [TestClass]
    public class AddMainSectionTests
    {
        private  const string MainSectionName = "Test trips";

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
        public void AddMainSection_WithPropeMainSection_ShouldAddOneMainSection()
        {
            // 1. Arrange            
            var mainSectionModel = new CreateMainSectionBindingModel()
            {
                Name = MainSectionName,
            };

            // 2. Act
            this.service.AddMainSection(mainSectionModel);

            // 3. Asserts 
            Assert.AreEqual(1, this.dbContext.MainSections.Count());
            
        }

        [TestMethod]
        public void AddMainSection_WithPropeрMainSection_ShouldAddCorrectly()
        {
            // 1. Arrange            
            var mainSectionModel = new CreateMainSectionBindingModel()
            {
                Name = MainSectionName,
            };

            // 2. Act
            this.service.AddMainSection(mainSectionModel);

            // 3. Asserts            
            var mainSection = this.dbContext.MainSections.First();
            Assert.AreEqual(MainSectionName, mainSection.Name);
        }

        [TestMethod]
        public void AddMainSection_WithNullMainSection_ShouldThrowException()
        {
            // 1. Arrange           
            CreateMainSectionBindingModel mainSectionModel = null;

            // 2. Act
            Action addMainSection = () => this.service.AddMainSection(mainSectionModel);

            // 3. Asserts 
            Assert.ThrowsException<ArgumentNullException>(addMainSection);
        }
    }
}
