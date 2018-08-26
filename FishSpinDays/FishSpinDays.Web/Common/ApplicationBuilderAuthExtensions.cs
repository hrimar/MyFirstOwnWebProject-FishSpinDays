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

            var rodPublication = new Publication()
            {
                SectionId = 3,
                Author = author,
                CreationDate = DateTime.Now - TimeSpan.FromDays(30),
                Title = "How to protect your sticks",
                Description =
                    "<img src=\"https://www.fishing.net.nz/default/assets/Image/2017/protect-your-sticks-08-558.jpg\" style =\"width: 40%; float: left;\">"+
                    "When rods fail, it is not always the blank that breaks; sometimes components fail or the construction methods are inadequate." +
"Component failure was the cause of one hilarious(at least to the onlookers) episode that took place near White Island.We were jigging for kingfish, and one of the team, who shall remain nameless, was using a new jig rod with a plastic gimbal nock." +
    "He hooked up on a good fish, jammed the rod butt into his gimbal belt, and being a powerful young chap, put some serious pressure on the kingie to try and get its head up.Almost immediately, the plastic gimbal nock’s two bottom tangs snapped off, allowing the rod butt to pop out of the gimbal belt and fly up to smack the angler in the face with the reel, hard, with his chin also knocking it out of gear. The resulting backlash was of Bob Marley proportions, jamming the reel and bringing the rod slamming down on the gunwale, the line instantly snapping!"+
"This all took place in a handful of seconds and was one of the finest pieces of fishing slapstick I have ever witnessed, all thanks to that rod company trying to save a couple of bucks by using a cheap, inadequate rod component."+
"The eyes have it"+
"Rod guides are always at risk, as they protrude out from the rod.A couple of things you can do include: avoid knocking the guides around in general, and if you use the guides as a spot to secure a hook or lure, place the hook through the side of the guide frame, not through the ceramic ring, to avoid damaging or popping the ring out of the frame."+
"Guides can also be damaged when people use them to get a good grip on a rod when trying to twist free a stuck ferrule.Some rods incorporate a section of nonslip finish near the ferrule to improve your grip, which is great if the line doesn’t touch it under pressure.When ferrules are badly stuck, I use a device called a grooming glove. These are a type of rubber mitt with small protrusions, used for grooming horses, and may be cheaply bought from equestrian supply stores.They allow a really strong, non - slip grip to be made on the rod without holding onto the guides."+
"The feet of rod guides are often ground to a gradual slope so the binding thread can climb smoothly up the foot when the guide’s being bound to the rod.If the grinding of the guide foot is done carelessly, it can leave a metal burr hanging off the end, which digs into the blank when the binding is done.You can’t see this, as it is hidden under the binding, but if your rod breaks exactly at the end of the guide foot, this is a possible cause.A layer of protective binding between the guide foot and the blank(called ‘underbinding’) can help protect the rod, but also adds weight to light rods and can make the action a bit soggy."+
"Most rods are designed to be specifically fished ‘over’ or ‘under’. ‘Over’ rods, such as baitcasters, game rods and other rods where the reel is used on top of the rod, often have double-footed guides, usually ‘bridge’ types that have further bracing between the two feet.Naturally, such a guide is stiffer than the rod, but because each guide foot bends away from the rod when under load, it does not dig into the blank. Guides on these types of rod are often underbound for added protection, but if used upside down(i.e.with the guides under the rod) the stiffness of the bridged guides may cause them to dig into the rod blank and damage it, potentially leading to eventual breakage. The guides on most rods designed to fish ‘under’ are usually single - foot guides with no bracing(except sometimes the bottom ‘stripping’ guide), so they bend with the rod."+
"Down the tubes"+
"Any angler who travels regularly with their own tackle is likely to have a horror story to tell about damage or drama involving their rods."+
"I have plenty myself. Without rod tubes – sturdy plastic or aluminium tubes to protect your rods when travelling – it is highly unlikely they will get to the destination undamaged.Indeed, some of the ‘baggage throwers’ at airports seem to treat my various rod tubes with their ‘fragile’ stickers as a particular challenge. One well-travelled tube is on its fifth set of end caps, and one airline managed a notable double on a trip to Norfolk Island, breaking the top of the tube on the way up and the base on the way home. Fortunately, the rods all survived."+
"Professionally - made tubes, such as those by Plano and Flambeau, are often adjustable for length in a telescopic fashion.I have one such tube that has travelled the world for over two decades.In this time I have had the end caps smashed four times(I replace them with heavy plumbing end - caps now).Tape is your friend: tape on the end caps, and be sure to tape over the adjustment catch for the telescopic extension."+
"The latter advice is the result of another bad experience, with the adjustment catch popping open in transit, allowing the tube to be jammed down until the rods popped the end out, with five being broken. Some kind airline employee replaced the broken rods in the tube, shut it all up again and kept quiet about it.I only discovered the damage upon unpacking the tube a week later, by which stage the airline refused any liability as I hadn’t reported it immediately."+
"But this experience aside, judging from the damage my tubes have suffered over the years, they have saved the lives of many of my rods which, after all, is what they are there for."+
"You can make your own one - piece rod tube out of plastic pipe(heavy stuff, not lightweight downpipe) and matching plumbing fittings such as end caps and inspection caps.I glue and pop - rivet a cap on one end and either use a screw - on inspection cap or press - on end cap on the other end.Either way, tape them closed(use lots of tape), but drill a small hole in one end so they don’t pressurise in flight and get jammed on.Don’t fit a lock, as airline security can get snaky about it, especially the Americans, who have on occasion smashed open the tube’s end cap, even when it was only taped on to enable easy inspection."+
"When packing the rods into a tube for a trip, pad the former by putting them in rod bags or wrapping them with clothing such as spare socks or T - shirts, then tape or strap them all together(there is strength in unity).Finally, pad the end / s of the bundle so the rods cannot slide up and down inside the tube and mash a tip under rough handling.Most of the damage done to my rods has been at the hands of airlines."+
"When transporting rods, compactness is an advantage. Many of my spin rods are two - piece models, the advantages for stowage and transport being obvious.Multi - piece(three to five pieces usually) ‘backpacker’ rods can be taken down even shorter and may fit inside a pack or bag, further protecting them, and also avoiding airline charges for an extra piece of baggage if transported in a separate tube."+
"Finally, if travelling off the beaten track, a small rod-and - reel repair kit comes along with me. This enables guides and tips to be replaced, basic reel servicing to be made, loose gimbal nocks and reel seats to be glued etc, and has saved the day for me and fellow anglers on many occasions, especially in more remote areas."+
"Keep it clean"+
"Finally, after use, rods should get a wash in the shower.If you are bait fishing, especially with baits such as skipjack or pilchards, your hands can transfer a lot of muck to the grips.This makes them look and smell bad, become slippery, and the smell can attract rodents(or pets), which may chew them up.Salt will also add to corrosion issues with metal fittings.After washing all the salt off the rod, I use a green pot - scourer to quickly scrub the grips down with the help of a little soap or shampoo … because they’re worth it!"
            };

            context.Publications.Add(rodPublication);
            context.SaveChanges();
        }
    }
}
