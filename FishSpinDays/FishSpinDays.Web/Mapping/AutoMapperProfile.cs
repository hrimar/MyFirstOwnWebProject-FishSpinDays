﻿namespace FishSpinDays.Web.Mapping
{
using AutoMapper;
    using FishSpinDays.Common.Admin.BindingModels;
    using FishSpinDays.Common.Admin.ViewModels;
    using FishSpinDays.Common.Identity.ViewModels;
    using FishSpinDays.Models;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            this.CreateMap<User, UserShortViewModel>();
            this.CreateMap<User, UserDetailsViewModel>();

            this.CreateMap<CreateSectionBindingModel, Section>();
            this.CreateMap<Section, SectionShortViewModel>();

            this.CreateMap<Section, SectionDetailsViewModel>();
            this.CreateMap<Publication, PublicationViewModel>()
                .ForMember(lvm => lvm.Section, option => option.MapFrom(src => src.Section.Name));

            this.CreateMap<Comment, CommentViewModel>();

            //this.CreateMap<InstancesCreationBindingModel, CourseInstance>();
            //this.CreateMap<LectureCreatingBindingModel, Lecture>();

            //// If the mane of the prop-s are different:
            //this.CreateMap<User, CommentViewModel>()
            //    .ForMember(lvm => lvm.Name, option => option.MapFrom(src => src.UserName));
        }

    }
}