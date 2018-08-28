namespace FishSpinDays.Services.Identity.Interfaces
{
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;


    public interface IIdentityService
    {
        IEnumerable<SelectListItem> GettAllSections();

        Section GetSection(int id);

        Publication CreatePublication(User author, Section section, string title, string description);

        Publication GetPublicationById(int id);

        bool IsLikedPublication(Publication publication);

        Comment AddComment(User author, Publication publication, string text);

        Comment GetCommentById(int id);

        bool IsLikedComment(Comment comment);

        bool IsUnLikedComment(Comment comment);

        User GetUserById(string id);

        List<SearchPublicationViewModel> FoundPublications(string searchTerm);
    }
}
