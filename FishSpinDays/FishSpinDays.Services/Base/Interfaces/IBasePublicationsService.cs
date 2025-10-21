namespace FishSpinDays.Services.Base.Interfaces
{
    using FishSpinDays.Common.Base.ViewModels;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBasePublicationsService
    {
        IEnumerable<PublicationShortViewModel> GetAllPublications(int page, int results);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsAsync(int page, int results, CancellationToken cancellationToken = default);

        PublicationViewModel GetPublication(int id);
        Task<PublicationViewModel> GetPublicationAsync(int id, CancellationToken cancellationToken = default);

        PublicationViewModel MostReaded();
        Task<PublicationViewModel> MostReadedAsync(CancellationToken cancellationToken = default);

        IEnumerable<PublicationShortViewModel> GetAllSeaPublications(int page, int count);
        Task<IEnumerable<PublicationShortViewModel>> GetAllSeaPublicationsAsync(int page, int count, CancellationToken cancellationToken = default);

        IEnumerable<PublicationShortViewModel> GetAllFreshwaterPublications(int page, int count);
        Task<IEnumerable<PublicationShortViewModel>> GetAllFreshwaterPublicationsAsync(int page, int count, CancellationToken cancellationToken = default);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisSection(string sectionType);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisSectionAsync(string sectionType, CancellationToken cancellationToken = default);

        int TotalPublicationsCount();
        Task<int> TotalPublicationsCountAsync(CancellationToken cancellationToken = default);

        int TotalPublicationsCount(string type);
        Task<int> TotalPublicationsCountAsync(string type, CancellationToken cancellationToken = default);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisYear(int year);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisYearAsync(int year, CancellationToken cancellationToken = default);

        IEnumerable<PublicationShortViewModel> GetAllPublicationsInThisMonth(int month);
        Task<IEnumerable<PublicationShortViewModel>> GetAllPublicationsInThisMonthAsync(int month, CancellationToken cancellationToken = default);
    }
}
