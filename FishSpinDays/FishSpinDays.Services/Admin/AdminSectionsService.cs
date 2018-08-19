using AutoMapper;
using FishSpinDays.Common.Admin.BindingModels;
using FishSpinDays.Data;
using FishSpinDays.Models;
using FishSpinDays.Services.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FishSpinDays.Services.Admin
{
   public class AdminSectionsService : BaseService, IAdminSectionsService
    {
        public AdminSectionsService(FishSpinDaysDbContext dbContex, IMapper mapper) 
            : base(dbContex, mapper)
        {  }

        public Section AddCourse(CreateSectionBindingModel model)
        {
            var course = this.Mapper.Map<Section>(model);
             this.DbContext.Sections.Add(course);
             this.DbContext.SaveChanges();

            return course;
        }
    }
}
