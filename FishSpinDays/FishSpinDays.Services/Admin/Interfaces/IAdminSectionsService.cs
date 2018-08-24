namespace FishSpinDays.Services.Admin.Interfaces
{
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Identity.ViewModels;
    using FishSpinDays.Models;
    using System.Collections.Generic;

    public interface IAdminSectionsService
    {
        Section AddSection(CreateSectionBindingModel model);

        MainSection AddMainSection(CreateMainSectionBindingModel model);

        IEnumerable<SectionShortViewModel> GetSections();

        SectionDetailsViewModel SectionDetails(int id);

        IEnumerable<MainSectionShortViewModel> GetMainSections();

        MainSectionDetailsViewModel MainSectionDetails(int id);

        CreateSectionBindingModel PrepareSectionForCreation(int mainSectioId);                    

    }
}
