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

                //-----seed of publication:
                if (!context.Publications.Any())
                {
                    SeedPublication(context);
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

        private static void SeedPublication(FishSpinDaysDbContext context)
        {
            var author = context.Users.FirstOrDefault(u => u.UserName == "admin");

            var seaPublication = new Publication()
            {
                SectionId = 1,
                Author = author,
                Title = "Sardinia: Holy Land",
                 Description = @"How many times have we heard call our island HOLY LAND … how many times have we smug smile at the name aware of some nice catches … but how many times have we cursed his marine sterility … Often the holy land has asked “our blood”, night sleep of a few hours, cold water, burning sun, wind and launches with an average of missed catch really embarrassing. That’s why the holy land has defined as such, needs sacrifice …The days of easy catches are gone, indeed ever known. This report will be a tribute of images and colors to my land surrounded by some “fish” few words but many pictures. Unfortunately I go fishing only with my phone (in addition to rod and lures), otherwise the spectacle of nature, with a camera, would have come out in all its glory .." +
            "<img src=\"http://www.seaspin.com/eng/wp-content/uploads/2015/11/TS32-.jpg\" style=\"width: 40%; float: left;\">" +
            "The beauty of sunrises and sunsets … that is the holy land!!!!Nature gives us details of which I didn’t even know the existence.Moving on cliffs or through vegetation often happens to come across creatures that seem to come from the Amazon rainforest.Let’s say that the variety of fish is not lacking," +
                "from small amberjack to greenhouse, from sea bass to dorado, from bonitos to the barracudas …" +
               "This year every corner of the coast for a few days was registered in green gold, even dolphinfish lost its charm for the ease with which you could catch it.Last year or two years ago however very few managed so… I still remember well the bad feeling I had seeing them follow any kind of lure without attacking: I could have smashed the rod on the rocks.Then the choice of fishing with smaller baits sometimes saved me from a washout, making me go home with a pretty stupid smile on my face.",
            };

            context.Publications.Add(seaPublication);
            context.SaveChanges();           
        }
    }
}
