using AutoMapper;
using FishSpinDays.Data;
using FishSpinDays.Models;
using FishSpinDays.Services.Admin;
using FishSpinDays.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FishSpinDays.Tests.Services.AdminSections
{
    [TestClass]
    public class GetMainSectionsTests
    {        
        private FishSpinDaysDbContext dbContext;
        private IMapper mapper;

        [TestInitialize]
        public void InitilizeTests() 
        {
            this.dbContext = MockDbContext.GetContext();
            this.mapper = MockAutomapper.GetMapper();
        }

        [TestMethod]
        public void GetMainSections_WithAFewMainSections_ShouldReturnNotNull()
        {
            // 1. Arrange
            this.dbContext.MainSections.Add(new MainSection() { Id = 1, Name = "First Main Section" });
            this.dbContext.MainSections.Add(new MainSection() { Id = 2, Name = "Second Main Section" });
            this.dbContext.MainSections.Add(new MainSection() { Id = 3, Name = "Third MainS ection" });
            this.dbContext.SaveChanges();

            var service = new AdminSectionsService(dbContext, this.mapper);

            // 2. Act
            var mainSection = service.GetMainSections();

            // 3. Assert
            Assert.IsNotNull(mainSection);          
        }
        
        [TestMethod]
        public void GetMainSections_WithAFewMainSections_ShouldReturnAll()
        {
            // 1. Arrange
            this.dbContext.MainSections.Add(new MainSection() { Id = 1, Name = "First Main Section" });
           this.dbContext.MainSections.Add(new MainSection() { Id = 2, Name = "Second Main Section" });
           this.dbContext.MainSections.Add(new MainSection() { Id = 3, Name = "Third MainS ection" });
           this.dbContext.SaveChanges();

            var service = new AdminSectionsService(dbContext, this.mapper);

            // 2. Act
            var mainSection = service.GetMainSections();

            // 3. Assert
            Assert.AreEqual(3, mainSection.Count());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, mainSection.Select(c => c.Id).ToArray());
        }


        [TestMethod]
        public void GetMainSections_WithNoMainSections_ShouldReturnNone()
        {
            // 1. Arrange           
            var service = new AdminSectionsService(dbContext, this.mapper);

            // 2. Act
            var mainSection =  service.GetMainSections();

            // 3. Assert
            Assert.IsNotNull(mainSection);
            Assert.AreEqual(0, mainSection.Count());
        }

    }
}
