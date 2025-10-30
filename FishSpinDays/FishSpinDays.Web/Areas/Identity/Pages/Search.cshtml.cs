namespace FishSpinDays.Web.Areas.Identity.Pages
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FishSpinDays.Common.Base.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using FishSpinDays.Services.Identity.Interfaces;
    using System.Threading.Tasks;

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

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(this.SearchTerm))
            {
                return;
            }

            var foundPublications = await this.IdentityService.FoundPublicationsAsync(this.SearchTerm).ConfigureAwait(false);

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