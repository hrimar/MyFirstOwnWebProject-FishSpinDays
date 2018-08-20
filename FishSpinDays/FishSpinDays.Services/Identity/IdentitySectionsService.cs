namespace FishSpinDays.Services.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using FishSpinDays.Common.Identity;
    using FishSpinDays.Common.Identity.ViewModels;
    using FishSpinDays.Data;
    using FishSpinDays.Services.Identity.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class IdentitySectionsService : BaseService, IIdentitySectionsService
    {
        public IdentitySectionsService(FishSpinDaysDbContext dbContex, IMapper mapper)
            : base(dbContex, mapper)
        { }

        public IEnumerable<SectionShortViewModel> GetSections()
        {
            var courses = this.DbContext.Sections.ToList();
            var model = this.Mapper.Map<IEnumerable<SectionShortViewModel>>(courses);
            return model;
        }

        public SectionDetailsViewModel SectionDetails(int id)
        {
            var section =  this.DbContext.Sections
                 .Include(s=>s.Publications)
                 .ThenInclude(s => s.Author)
                 .FirstOrDefault(s => s.Id == id);

            SectionDetailsViewModel model = this.Mapper.Map<SectionDetailsViewModel>(section);
            return model;
        }
    }
}
