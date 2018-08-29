namespace FishSpinDays.Services.Base
{
    using AutoMapper;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Common.Identity.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Base.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using FishSpinDays.Common.Extensions;
    using FishSpinDays.Common.Base.ViewModels;

    public class BasePublicationsService : BaseService, IBasePublicationsService
    {
        public BasePublicationsService(FishSpinDaysDbContext dbContex, IMapper mapper)
            : base(dbContex, mapper)
        { }

        public IEnumerable<PublicationShortViewModel> GetAllPublications(int page, int count)
        {
            if (page <= 0)
            {
                return null;
            }

            var publications = this.DbContext.Publications
                .OrderByDescending(p => p.CreationDate)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            var model = publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = GetAutorById(p.AuthorId),
                Description = p.Description.GetOnlyTextFromDescription(),
                CoverImage = GetMainImage(p.Description)
            })
            .ToList();

            return model;
        }

        public PublicationViewModel GetPublication(int id)
        {
            var publication = this.DbContext.Publications
                .Include(s => s.Comments)
                 .Include(s => s.Author)
                 .Include(s => s.Section)
                 .FirstOrDefault(s => s.Id == id);

            var model = this.Mapper.Map<PublicationViewModel>(publication);

            return model;
        }

        public PublicationViewModel MostReaded()
        {
            var publication = this.DbContext.Publications
                .OrderByDescending(p => p.Likes)
                .Include(s => s.Comments)
                 .Include(s => s.Author)
                 .Include(s => s.Section)
                 .First();

            var model = this.Mapper.Map<PublicationViewModel>(publication);

            return model;
        }


        public IEnumerable<PublicationShortViewModel> GetAllSeaPublications(int page, int count)
        {
            List<Publication> publications = GetTargetSection(WebConstants.SeaSection, page, count);
            if (publications == null)
            {
                return null;
            }
            List<PublicationShortViewModel> model = GetAllTargetPublications(publications);

            return model;
        }


        public IEnumerable<PublicationShortViewModel> GetAllFreshwaterPublications(int page, int count)
        {
            List<Publication> publications = GetTargetSection(WebConstants.FreshwaterSection, page, count);
            if (publications == null)
            {
                return null;
            }
            List<PublicationShortViewModel> model = GetAllTargetPublications(publications);

            return model;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisSection(string sectionType)
        {
            List<Publication> publications = this.DbContext.Publications
                            .Where(p => p.Section.Name == sectionType)
                            .Take(WebConstants.DefaultResultPerPage)
                            .ToList();
            List<PublicationShortViewModel> model = GetAllTargetPublications(publications);

            return model;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisYear(int year)
        {
            List<Publication> publications = this.DbContext.Publications
                            .Where(p => p.CreationDate.Year == year)
                            .Take(WebConstants.DefaultResultPerPage)
                            .ToList();
            List<PublicationShortViewModel> model = GetAllTargetPublications(publications);

            return model;
        }

        public IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisMonth(int month)
        {
            List<Publication> publications = this.DbContext.Publications
                           .Where(p => p.CreationDate.Month == month)
                           .Take(WebConstants.DefaultResultPerPage)
                           .ToList();
            List<PublicationShortViewModel> model = GetAllTargetPublications(publications);

            return model;
        }


        private List<PublicationShortViewModel> GetAllTargetPublications(List<Publication> publications)
        {
            return publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = GetAutorById(p.AuthorId),
                Description = p.Description.GetOnlyTextFromDescription(),
                CoverImage = GetMainImage(p.Description)
            })
            .ToList();
        }


        private List<Publication> GetTargetSection(string targetSection, int page, int count)
        {
            if (page <= 0)
            {
                return null;
            }

            return this.DbContext.Publications
                            .Where(p => p.Section.Name == targetSection)
                            .OrderByDescending(p => p.CreationDate)
                            .Skip((page - 1) * count)
                            .Take(count)
                            .ToList();
        }

        private string GetAutorById(string authorId)
        {
            User author = this.DbContext.Users.FirstOrDefault(a => a.Id == authorId);

            return author.UserName;
        }

        private string GetMainImage(string description)
        {
            var regex = new Regex("<img[^>]+src=\"([^\">]+)\"");
            var matched = regex.Match(description);

            string image = string.Empty;
            if (matched.Success)
            {
                image = matched.Groups[1].Value;
            }

            return image;
        }

        public int TotalPublicationsCount()
        {
            return this.DbContext.Publications.Count();
        }

        public int TotalPublicationsCount(string type)
        {
            return this.DbContext.Publications
                .Include(p => p.Section)
                .Where(p => p.Section.Name == type)
                .Count();
        }
              
    }
}
