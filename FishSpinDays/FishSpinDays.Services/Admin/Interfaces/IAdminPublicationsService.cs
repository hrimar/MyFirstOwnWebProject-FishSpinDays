namespace FishSpinDays.Services.Admin.Interfaces
{
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Models;

    public interface IAdminPublicationsService
    {
        PublicationBindingModel GetPublication(int id);

        Publication EditPublication(PublicationBindingModel model);

        bool DeleteComment(int id);
    }
}
