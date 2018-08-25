namespace FishSpinDays.Services.Admin
{
    using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Validation;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Admin.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class AdminPublicationsService : BaseService, IAdminPublicationsService
    {
        public AdminPublicationsService(FishSpinDaysDbContext dbContex, IMapper mapper)
            : base(dbContex, mapper)
        { }
             

        public PublicationBindingModel GetPublication(int id)
        {
            var publiction = this.DbContext.Publications
                 .Include(p => p.Author)
                 .Include(p => p.Section)
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
