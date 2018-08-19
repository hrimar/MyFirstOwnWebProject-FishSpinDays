using FishSpinDays.Common.Identity.ViewModels;
using System;
using System.Collections.Generic;

namespace FishSpinDays.Services.Base.Interfaces
{
   public interface IBasePublicationsService
    {
        IEnumerable<PublicationShortViewModel> GetAllPublications();
    }
}
