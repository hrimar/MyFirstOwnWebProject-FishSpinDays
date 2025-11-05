namespace FishSpinDays.Common.API.Models.Publications
{
    public class PublicationStatsResponse
    {
        public int TotalPublications { get; set; }
        public int SeaPublications { get; set; }
        public int FreshwaterPublications { get; set; }
        public int OtherPublications { get; set; }
    }
}
