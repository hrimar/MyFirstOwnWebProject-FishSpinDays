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
using System.Threading.Tasks;

namespace FishSpinDays.Services.Base
{
    public class BasePublicationsService : BaseService, IBasePublicationsService
    {
        public BasePublicationsService(FishSpinDaysDbContext dbContex, IMapper mapper)
            : base(dbContex, mapper)
        { }

        public IEnumerable<PublicationShortViewModel> GetAllPublications()
        {
            var publications = this.DbContext.Publications.ToList();

            var model = publications.Select(p => new PublicationShortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Author = GetAutorById(p.AuthorId),
                Description = GetOnlyTextFromDescription(p.Description),
                CoverImage = GetMainImage(p.Description)
            })
            .ToList();

            return model;
        }

        public PublicationViewModel GetPublication(int id)
        {
            var publication =  this.DbContext.Publications
                .Include(s => s.Comments)
                 .Include(s => s.Author)
                 .Include(s=>s.Section)
                 .FirstOrDefault(s => s.Id == id);
            
            
            var model = this.Mapper.Map<PublicationViewModel>(publication);
                       
            return model;
        }



        private string GetOnlyTextFromDescription(string description)
        {
            //var template = @"<img.*?\\?.*?>";
            //var regex = new Regex(template);
            //var matched = regex.Match(description);

            //var image = matched.Groups[0].Value;
            //var result = Regex.Replace(description, template, "");

            //string shortResult = Truncate(result, WebConstants.DescriptinMaxLength);
            //return shortResult;

            var imgTemplate = @"<img.*?\\?.*?>";

            var regex = new Regex(imgTemplate);
            var matched = regex.Match(description);

            var image = matched.Groups[0].Value;
            var result = Regex.Replace(description, imgTemplate, "");

            var youtubeTemplate = @"<iframe.*?<\/iframe>";

            var regexY = new Regex(imgTemplate);
            var matchedY = regexY.Match(result);

            var youtube = matched.Groups[0].Value;
            var finalResult= Regex.Replace(result, youtubeTemplate, "");

            string shortResult = Truncate(finalResult, WebConstants.DescriptinMaxLength);
            return shortResult;
        }

        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
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

        
       
    }
}
