namespace FishSpinDays.Common.API.Models.Publications
{
    using System.Collections.Generic;

    public class PartPublicationsResponseModel
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public IEnumerable<PublicationShortResponseModel> Publications { get; set; }
    }
}