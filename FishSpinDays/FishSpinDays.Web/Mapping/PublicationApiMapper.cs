namespace FishSpinDays.Web.Mapping
{
    using FishSpinDays.Common.API.Models.Publications;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Models;

    /// <summary>
    /// Manual mapper for Publication entities to API response models
    /// This avoids AutoMapper circular reference issues with Comment collections
    /// </summary>
    public static class PublicationApiMapper
    {
        public static PublicationShortResponseModel ToShortResponseModel(Publication publication)
        {
            if (publication == null) return null;

            return new PublicationShortResponseModel
            {
                Id = publication.Id,
                Title = publication.Title,
                CreationDate = publication.CreationDate,
                Likes = publication.Likes,
                Author = publication.Author?.UserName,
                Section = publication.Section?.Name,
                CommentsCount = publication.Comments?.Count ?? 0
            };
        }

        public static PublicationResponseModel ToResponseModel(Publication publication)
        {
            if (publication == null) return null;

            return new PublicationResponseModel
            {
                Id = publication.Id,
                Title = publication.Title,
                Description = publication.Description,
                CreationDate = publication.CreationDate,
                Likes = publication.Likes,
                Author = publication.Author?.UserName,
                AuthorId = publication.AuthorId,
                Section = publication.Section?.Name,
                CommentsCount = publication.Comments?.Count ?? 0
            };
        }

        public static PublicationResponseModel ToResponseModel(PublicationViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new PublicationResponseModel
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                Description = viewModel.Description,
                CreationDate = viewModel.CreationDate,
                Likes = viewModel.Likes,
                Author = viewModel.Author,
                AuthorId = viewModel.AuthorId,
                Section = viewModel.Section,
                CommentsCount = viewModel.Comments?.Count ?? 0
            };
        }

        public static PublicationShortResponseModel ToShortResponseModel(PublicationShortViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new PublicationShortResponseModel
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                CreationDate = viewModel.CreationDate,
                Author = viewModel.Author,
                Likes = 0,
                Section = null,
                CommentsCount = 0
            };
        }

        public static SearchPublicationResponseModel ToSearchResponseModel(SearchPublicationViewModel viewModel)
        {
            if (viewModel == null) return null;

            return new SearchPublicationResponseModel
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                SearchResult = viewModel.SearchResult
            };
        }
    }
}
