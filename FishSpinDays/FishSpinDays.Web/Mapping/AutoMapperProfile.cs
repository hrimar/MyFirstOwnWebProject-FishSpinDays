namespace FishSpinDays.Web.Mapping
{
    using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Common.API.Models.Comments;
    using FishSpinDays.Common.API.Models.Publications;
    using FishSpinDays.Common.API.Models.Sections;
    using FishSpinDays.Common.Base.ViewModels;
    using FishSpinDays.Common.Identity.ViewModels;
    using FishSpinDays.Models;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<User, UserShortViewModel>();
            this.CreateMap<User, FishSpinDays.Common.Admin.ViewModels.UserDetailsViewModel>();
            this.CreateMap<User, FishSpinDays.Common.Identity.ViewModels.UserDetailsViewModel>();
            this.CreateMap<Publication, PublicationConciseViewModel>();

            this.CreateMap<CreateMainSectionBindingModel, MainSection>();
            this.CreateMap<CreateSectionBindingModel, Section>();
            this.CreateMap<Section, SectionShortViewModel>();
            this.CreateMap<MainSection, MainSectionShortViewModel>();

            this.CreateMap<Section, SectionDetailsViewModel>();
            this.CreateMap<MainSection, MainSectionDetailsViewModel>();

            this.CreateMap<Publication, PublicationShortViewModel>();
            this.CreateMap<Publication, PublicationViewModel>()
                .ForMember(lvm => lvm.Section, option => option.MapFrom(src => src.Section.Name));

            this.CreateMap<Comment, CommentViewModel>();

            // NOTE: API DTO mappings are handled by manual mappers in FishSpinDays.Common.API.Mappers
            // to avoid AutoMapper circular reference issues with Comment collections
            // See PublicationApiMapper.cs for manual mapping implementations

            this.CreateMap<SearchPublicationViewModel, SearchPublicationResponseModel>();
        }
    }
}
