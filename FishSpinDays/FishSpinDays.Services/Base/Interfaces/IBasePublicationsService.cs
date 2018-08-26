using FishSpinDays.Common.Base.ViewModels;
using FishSpinDays.Common.Identity.ViewModels;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FishSpinDays.Services.Base.Interfaces
{
    public interface IBasePublicationsService
    {
        IEnumerable<PublicationShortViewModel> GetAllPublications(int page, int results);

        PublicationViewModel GetPublication(int id);

        PublicationViewModel MostReaded();

        IEnumerable<PublicationShortViewModel> GetAllSeaPublications(int page, int count);

        IEnumerable<PublicationShortViewModel> GetAllFreshwaterPublications(int page, int count);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisSection(string sectionType);
               
        int TotalPublicationsCount();

        int TotalPublicationsCount(string type);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisYear(int year);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisMonth(int month);
    }
}
