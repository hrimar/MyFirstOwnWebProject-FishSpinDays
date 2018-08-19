using FishSpinDays.Common.Identity.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FishSpinDays.Services.Base.Interfaces
{
   public interface IBasePublicationsService
    {
        IEnumerable<PublicationShortViewModel> GetAllPublications();

       Task<PublicationViewModel> GetPublication(int id);
    }
}
