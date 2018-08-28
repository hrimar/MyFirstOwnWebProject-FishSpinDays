namespace FishSpinDays.Services.Identity
{
    using AutoMapper;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FishSpinDays.Common.Extensions;

    public class IdentityService : BaseService, IIdentityService
    {
        
        public IdentityService(FishSpinDaysDbContext dbContex, IMapper mapper) 
            : base(dbContex, mapper)
        {  }

        public Section GetSection(int id)
        {
            return this.DbContext.Sections.Find(id);
        }

        public Publication CreatePublication(User author, Section section, string title, string description)
        {
            Publication publication = new Publication()
            {
                Author = author,
                Section = section,
                Title = title,
                Description = description
            };

            try
            {
                this.DbContext.Publications.Add(publication);
                this.DbContext.SaveChanges();

                return publication;
            }
            catch
            {
                return null;
            }
        }

        public Publication GetPublicationById(int id)
        {
           return this.DbContext.Publications.FirstOrDefault(p => p.Id == id);
        }

        public bool IsLikedPublication(Publication publication)
        {
            publication.Likes++;
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

        public Comment AddComment(User author, Publication publication, string text)
        {
            Comment comment = new Comment()
            {
                Author = author,
                Publication = publication,
                Text = text
            };

            try
            {
                this.DbContext.Comments.Add(comment);
                this.DbContext.SaveChanges();

                return comment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Comment GetCommentById(int id)
        {
            return this.DbContext.Comments.FirstOrDefault(p => p.Id == id);
        }

        public bool IsLikedComment(Comment comment)
        {
            comment.Likes++;
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

        public bool IsUnLikedComment(Comment comment)
        {
            comment.UnLikes++;
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

        public IEnumerable<SelectListItem> GettAllSections()
        {
            var sections = this.DbContext.Sections
                .Select(b => new SelectListItem()
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                })
                .ToList();

            return sections;
        }

        public User GetUserById(string id)
        {
            var user = this.DbContext.Users
                .Include(u => u.Publications)
                .ThenInclude(u => u.Comments)
                .FirstOrDefault(u => u.Id == id);

            return user;
        }

        public List<SearchPublicationViewModel> FoundPublications(string searchTerm)
        {
            var foundPublications = this.DbContext.Publications
               .Where(a => a.Description.ToLower().Contains(searchTerm.ToLower()))
               .OrderBy(a => a.CreationDate)
               .Select(a => new SearchPublicationViewModel()
               {
                   Id = a.Id,
                   SearchResult = a.Description.GetOnlyTextFromDescription(),
                   Title = a.Title
               })
               .ToList();

            return foundPublications;
        }

      
    }
}
