using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FishSpinDays.Common.Base.ViewModels;
using FishSpinDays.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FishSpinDays.Web.Areas.Identity.Pages
{
    public class SearchModel : BaseModel
    {
        public SearchModel(FishSpinDaysDbContext dbContex) 
            : base(dbContex)
        {  }

        public List<SearchPublicationViewModel> Publications { get; set; }

        [BindProperty(SupportsGet = true)] 
        public string SearchTerm { get; set; }

        public void OnGet(string name)
        {
            this.SearchTerm = this.Request.HttpContext.Request
                .QueryString.ToString().Split('=').Last();

            this.Publications = this.DbContext.Publications
                  .Where(a => a.Description.Contains(this.SearchTerm))
                  .Select(a => new SearchPublicationViewModel
                  {
                      Id = a.Id,
                      Description = a.Description
                  })
                  .ToList();
           
            foreach (var publication in this.Publications)
            {
                string markedResult = Regex.Replace(
                    publication.Description,
                    $"({Regex.Escape(this.SearchTerm)})",
                    match => $"<strong class=\"text-danger\">{match.Groups[0].Value}</strong>",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                publication.Description = markedResult;
            }
                       
        }
    }
}