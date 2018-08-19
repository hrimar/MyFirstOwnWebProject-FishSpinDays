namespace FishSpinDays.Services.Admin.Interfaces
{
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Models;

    public interface IAdminSectionsService
    {
        Section AddCourse(CreateSectionBindingModel model);
    }
}
