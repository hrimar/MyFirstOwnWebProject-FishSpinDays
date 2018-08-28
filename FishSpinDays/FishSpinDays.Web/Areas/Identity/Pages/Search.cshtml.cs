namespace FishSpinDays.Web.Areas.Identity.Pages
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FishSpinDays.Common.Base.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Services.Identity.Interfaces;

    public class SearchModel : BaseModel
    {
        public SearchModel(IIdentityService identityService)
            : base(identityService)
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
            
            var foundPublications = this.IdentityService.FoundPublications(this.SearchTerm);

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