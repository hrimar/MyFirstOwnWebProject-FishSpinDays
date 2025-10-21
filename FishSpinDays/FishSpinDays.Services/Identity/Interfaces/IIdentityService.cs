namespace FishSpinDays.Services.Identity.Interfaces
{
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface IIdentityService
    {
        IEnumerable<SelectListItem> GettAllSections();
        Task<IEnumerable<SelectListItem>> GetAllSectionsAsync();

        Section GetSection(int id);
        Task<Section> GetSectionAsync(int id);

        Section GetSectionByName(string sectionName);
        Task<Section> GetSectionByNameAsync(string sectionName);

        Publication CreatePublication(User author, Section section, string title, string description);
        Task<Publication> CreatePublicationAsync(User author, Section section, string title, string description);

        Publication GetPublicationById(int id);
        Task<Publication> GetPublicationByIdAsync(int id);

        bool IsLikedPublication(Publication publication);
        Task<bool> IsLikedPublicationAsync(Publication publication);

        Comment AddComment(User author, Publication publication, string text);
        Task<Comment> AddCommentAsync(User author, Publication publication, string text);

        Comment GetCommentById(int id);
        Task<Comment> GetCommentByIdAsync(int id);

        bool IsLikedComment(Comment comment);
        Task<bool> IsLikedCommentAsync(Comment comment);

        bool IsUnLikedComment(Comment comment);
        Task<bool> IsUnLikedCommentAsync(Comment comment);

        User GetUserById(string id);
        Task<User> GetUserByIdAsync(string id);

        List<SearchPublicationViewModel> FoundPublications(string searchTerm);
        Task<List<SearchPublicationViewModel>> FoundPublicationsAsync(string searchTerm);
    }
}
