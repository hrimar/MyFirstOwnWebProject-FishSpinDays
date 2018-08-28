namespace FishSpinDays.Services.Admin
{
    using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Admin.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;

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
            var section = this.DbContext.Sections.ToList();
            var model = this.Mapper.Map<IEnumerable<SectionShortViewModel>>(section);
            return model;
        }

        public SectionDetailsViewModel SectionDetails(int id)
        {
            var section = this.DbContext.Sections
                 .Include(s => s.Publications)
                 .ThenInclude(s => s.Author)
                 .FirstOrDefault(s => s.Id == id);

            if (section == null)
            {
                return null;
            }

            SectionDetailsViewModel model = this.Mapper.Map<SectionDetailsViewModel>(section);
            return model;
        }

        public MainSectionDetailsViewModel MainSectionDetails(int id) 
        {
            var mainSection = this.DbContext.MainSections
                 .Include(s => s.Sections)               
                 .FirstOrDefault(s => s.Id == id);

            if (mainSection == null)
            {
                return null;
            }

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

        public PublicationBindingModel GetPublication(int id) 
        {
           var publiction = this.DbContext.Publications
                .Include(p=>p.Author)
                .Include(p=>p.Section)
                 .FirstOrDefault(s => s.Id == id);

            if (publiction == null)
            {
                return null;
            }

            var model = new PublicationBindingModel()
            {
                Title = publiction.Title,
                Section = publiction.Section.Name,
                Description = publiction.Description,
                Author = publiction.Author.UserName
            };

            return model;
        }

        public Publication EditPublication(PublicationBindingModel model) 
        {
            var publication = this.DbContext.Publications
                .Include(p => p.Section)
                .FirstOrDefault(p => p.Id == model.Id);

            publication.Section.Name = model.Section;
            publication.Title = model.Title;
            publication.Description = model.Description;

            this.DbContext.Publications.Update(publication);
            this.DbContext.SaveChanges();

            return publication;
        }

        public bool DeleteComment(int id)
        {
            var comment = this.DbContext.Comments
                .SingleOrDefault(c => c.Id == id);
                       
            this.DbContext.Comments.Remove(comment);
            try
            {
                this.DbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }        
    }
}
