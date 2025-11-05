namespace FishSpinDays.Common.API.Models.Stats
{
    using System;
    using System.Collections.Generic;

    public class OverviewStatsResponse
    {
        public int TotalPublications { get; set; }
        public int SeaPublications { get; set; }
        public int FreshwaterPublications { get; set; }
        public int TacklePublications { get; set; }
        public int ActionsPublications { get; set; }
        public int CurrentYearPublications { get; set; }
        public int CurrentMonthPublications { get; set; }
        public int OtherPublications { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class SectionCountItem
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class TrendMonthlyItem
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class TrendYearlyItem
    {
        public int CurrentYear { get; set; }
        public int Count { get; set; }
        public int PreviousYear { get; set; }
    }

    public class TrendStatsResponse
    {
        public IEnumerable<TrendMonthlyItem> Monthly { get; set; }
        public TrendYearlyItem Yearly { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class PopularStatsResponse
    {
        public Publications.PublicationShortResponseModel MostRatedPublication { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}