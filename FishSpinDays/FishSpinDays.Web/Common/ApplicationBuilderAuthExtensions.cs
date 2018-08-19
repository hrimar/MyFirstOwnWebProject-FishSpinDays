namespace FishSpinDays.Web.Common
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using FishSpinDays.Common.Constants;
    using FishSpinDays.Models;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationBuilderAuthExtensions
    {

        private const string DefaultAdminPasswprd = WebConstants.DefaultAdminPassowrd; 

        private static readonly IdentityRole[] roles =
        {
            new IdentityRole(WebConstants.AdminRole)
        };

        // TODO: Use a dictionary (string-> user) for roles and users

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

            }
        }
    }
}
