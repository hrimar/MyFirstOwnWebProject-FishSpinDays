namespace FishSpinDays.Web.Common
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Models;
    using Microsoft.Extensions.DependencyInjection;
    using FishSpinDays.Data;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using FishSpinDays.Common.Admin.ViewModels;
    using System.Collections.Generic;

    public static class ApplicationBuilderAuthExtensions
    {

        private const string DefaultAdminPasswprd = WebConstants.DefaultAdminPassowrd;

        private static readonly IdentityRole[] roles =
        {
            new IdentityRole(WebConstants.AdminRole)
        };

        public static async void SeedDatabase(this IApplicationBuilder app)
        {
            var serviceFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var scope = serviceFactory.CreateScope();
            using (scope)
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        var result = await roleManager.CreateAsync(role);
                    }
                }

                var admin = await userManager.FindByNameAsync("admin");
                if (admin == null)
                {
                    admin = new User()
                    {
                        UserName = "admin",
                        Email = "admin@example.com"
                    };

                    var result = await userManager.CreateAsync(admin, DefaultAdminPasswprd);

                    result = await userManager.AddToRoleAsync(admin, roles[0].Name);
                }

                // -----seed of main sections and sections:
                var context = scope.ServiceProvider.GetService<FishSpinDaysDbContext>();

                context.Database.Migrate();

                if (!context.MainSections.Any())
                {
                    var jsonMainSections = File.ReadAllText(@"wwwroot\seedfiles\mainsections.json");
                    var mainSectionDtos = JsonConvert.DeserializeObject<MainSectionDto[]>(jsonMainSections);

                    SeedMainSections(context, mainSectionDtos);
                }

            }
        }

        private static void SeedMainSections(FishSpinDaysDbContext context, MainSectionDto[] mainSectionDtos)
        {
            var mainSectionToCreate = mainSectionDtos
                .Select(ms => new MainSection
                {
                    Name = ms.Name,
                    Sections = ms.Sections.Select(s => new Section
                    {
                        Name = s
                    }).ToArray()
                }).ToArray();

            context.MainSections.AddRange(mainSectionToCreate);
            context.SaveChanges();            
        }
    }
}
