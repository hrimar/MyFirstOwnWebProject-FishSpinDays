using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FishSpinDays.Common.Base.ViewModels;
using FishSpinDays.Common.Constants;
using FishSpinDays.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FishSpinDays.Web.Helpers;

namespace FishSpinDays.Web.Areas.Identity.Pages
{
    public class SearchModel : BaseModel
    {
        public SearchModel(FishSpinDaysDbContext dbContex) 
            : base(dbContex)
        {
            this.SearchResults = new List<SearchPublicationViewModel>();
        }

        public List<SearchPublicationViewModel> SearchResults { get; set; }

        [BindProperty(SupportsGet = true)] 
        public string SearchTerm { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                return;
            }

            var foundPublications = this.DbContext.Publications
                .Where(a => a.Description.ToLower().Contains(this.SearchTerm.ToLower()))
                .OrderBy(a => a.CreationDate)
                .Select(a => new SearchPublicationViewModel()
                {
                    Id = a.Id,                    
                    SearchResult = a.Description.GetOnlyTextFromDescription(),
                    Title = a.Title
                })
                .ToList();
           
            this.SearchResults.AddRange(foundPublications);
        
            foreach (var result in this.SearchResults)
            {
                string markedResult = Regex.Replace(
                    result.SearchResult,
                    $"({Regex.Escape(this.SearchTerm)})",
                    match => $"<strong class=\"text-danger\">{match.Groups[0].Value}</strong>",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
                result.SearchResult = markedResult;
            }            
        }
                       
    }
}