using AutoMapper;
using FishSpinDays.Common.Admin.BindingModels;
using FishSpinDays.Common.Admin.ViewModels;
using FishSpinDays.Common.Identity.ViewModels;
using FishSpinDays.Data;
using FishSpinDays.Models;
using FishSpinDays.Services.Admin.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishSpinDays.Services.Admin
{
   public class AdminSectionsService : BaseService, IAdminSectionsService
    {
        public AdminSectionsService(FishSpinDaysDbContext dbContex, IMapper mapper) 
            : base(dbContex, mapper)
        {  }

        public MainSection AddMainSection(CreateMainSectionBindingModel model)
        {
            var mainSection = this.Mapper.Map<MainSection>(model);
            this.DbContext.MainSections.Add(mainSection);
            this.DbContext.SaveChanges();

            return mainSection;
        }

        public Section AddSection(CreateSectionBindingModel model)
        {
            var section = this.Mapper.Map<Section>(model);
             this.DbContext.Sections.Add(section);
             this.DbContext.SaveChanges();

            return section;
        }

        public IEnumerable<MainSectionShortViewModel> GetMainSections()
        {
            var mainSections = this.DbContext.MainSections.ToList();
            var model = this.Mapper.Map<IEnumerable<MainSectionShortViewModel>>(mainSections);
            return model;
        }

        public IEnumerable<SectionShortViewModel> GetSections()
        {
            var courses = this.DbContext.Sections.ToList();
            var model = this.Mapper.Map<IEnumerable<SectionShortViewModel>>(courses);
            return model;
        }

        public SectionDetailsViewModel SectionDetails(int id)
        {
            var section = this.DbContext.Sections
                 .Include(s => s.Publications)
                 .ThenInclude(s => s.Author)
                 .FirstOrDefault(s => s.Id == id);

            SectionDetailsViewModel model = this.Mapper.Map<SectionDetailsViewModel>(section);
            return model;
        }

        public MainSectionDetailsViewModel MainSectionDetails(int id) //
        {
            var mainSection = this.DbContext.MainSections
                 .Include(s => s.Sections)               
                 .FirstOrDefault(s => s.Id == id);

            MainSectionDetailsViewModel model = this.Mapper.Map<MainSectionDetailsViewModel>(mainSection);
            return model;
        }

        public CreateSectionBindingModel PrepareSectionForCreation(int mainSectioId)
        {
            var mainSection = this.DbContext.MainSections.Find(mainSectioId);
            if (mainSection == null)
            {
                return null;
            }

            var model = new CreateSectionBindingModel()
            {
                MainSectionId = mainSection.Id
            };

            return model;
        }
    }
}
