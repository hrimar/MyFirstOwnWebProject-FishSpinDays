namespace FishSpinDays.Common.API.Models.Search
{
    using FishSpinDays.Common.Base.ViewModels;
    using System.Collections.Generic;

    /// <summary>
    /// Model for search results with pagination information
    /// </summary>
    public class SearchResultsModel
    {
        public string SearchTerm { get; set; }
        public int TotalResults { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<SearchPublicationViewModel> Results { get; set; }
    }
}