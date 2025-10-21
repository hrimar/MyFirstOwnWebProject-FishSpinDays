namespace FishSpinDays.Services.Identity.Interfaces
{
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IIdentityService
    {
        IEnumerable<SelectListItem> GettAllSections();
        Task<IEnumerable<SelectListItem>> GetAllSectionsAsync(CancellationToken cancellationToken = default);

        Section GetSection(int id);
        Task<Section> GetSectionAsync(int id, CancellationToken cancellationToken = default);

        Section GetSectionByName(string sectionName);
        Task<Section> GetSectionByNameAsync(string sectionName, CancellationToken cancellationToken = default);

        Publication CreatePublication(User author, Section section, string title, string description);
        Task<Publication> CreatePublicationAsync(User author, Section section, string title, string description, CancellationToken cancellationToken = default);

        Publication GetPublicationById(int id);
        Task<Publication> GetPublicationByIdAsync(int id, CancellationToken cancellationToken = default);

        bool IsLikedPublication(Publication publication);
        Task<bool> IsLikedPublicationAsync(Publication publication, CancellationToken cancellationToken = default);

        Comment AddComment(User author, Publication publication, string text);
        Task<Comment> AddCommentAsync(User author, Publication publication, string text, CancellationToken cancellationToken = default);

        Comment GetCommentById(int id);
        Task<Comment> GetCommentByIdAsync(int id, CancellationToken cancellationToken = default);

        bool IsLikedComment(Comment comment);
        Task<bool> IsLikedCommentAsync(Comment comment, CancellationToken cancellationToken = default);

        bool IsUnLikedComment(Comment comment);
        Task<bool> IsUnLikedCommentAsync(Comment comment, CancellationToken cancellationToken = default);

        User GetUserById(string id);
        Task<User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

        List<SearchPublicationViewModel> FoundPublications(string searchTerm);
        Task<List<SearchPublicationViewModel>> FoundPublicationsAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
