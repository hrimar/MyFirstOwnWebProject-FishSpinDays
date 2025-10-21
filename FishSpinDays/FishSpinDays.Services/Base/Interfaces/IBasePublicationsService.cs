using FishSpinDays.Common.Base.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FishSpinDays.Services.Base.Interfaces
{
    public interface IBasePublicationsService
    {
        IEnumerable<PublicationShortViewModel> GetAllPublications(int page, int results);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsAsync(int page, int results);

        PublicationViewModel GetPublication(int id);
        Task<PublicationViewModel> GetPublicationAsync(int id);

        PublicationViewModel MostReaded();
        Task<PublicationViewModel> MostReadedAsync();

        IEnumerable<PublicationShortViewModel> GetAllSeaPublications(int page, int count);
        Task<IEnumerable<PublicationShortViewModel>> GetAllSeaPublicationsAsync(int page, int count);

        IEnumerable<PublicationShortViewModel> GetAllFreshwaterPublications(int page, int count);
        Task<IEnumerable<PublicationShortViewModel>> GetAllFreshwaterPublicationsAsync(int page, int count);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisSection(string sectionType);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisSectionAsync(string sectionType);

        int TotalPublicationsCount();
        Task<int> TotalPublicationsCountAsync();

        int TotalPublicationsCount(string type);
        Task<int> TotalPublicationsCountAsync(string type);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisYear(int year);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisYearAsync(int year);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisMonth(int month);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisMonthAsync(int month);
    }
}
