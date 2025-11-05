namespace FishSpinDays.Common.API.Models.Search
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for advanced search with additional filters
    /// </summary>
    public class AdvancedSearchModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string SearchTerm { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Additional filter properties can be added here for future enhancements
        // public string Section { get; set; }
        // public DateTime? FromDate { get; set; }
        // public DateTime? ToDate { get; set; }
    }
}