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

        IEnumerable<PublicationShortViewModel> GetAllSeaPublications();
     }
}
