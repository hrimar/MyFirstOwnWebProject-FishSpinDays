using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FishSpinDays.Data;
using FishSpinDays.Models;
using FishSpinDays.Web.Common;
using AutoMapper;
using FishSpinDays.Services.Admin.Interfaces;
using FishSpinDays.Services.Admin;
using FishSpinDays.Services.Base.Interfaces;
using FishSpinDays.Services.Base;

namespace FishSpinDays.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddDbContext<FishSpinDaysDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("FishSpinDays"),
                          dbOptions => dbOptions.MigrationsAssembly("FishSpinDays.Data")));

            services.AddDbContext<FishSpinDaysDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            LoginFromOtherApps(services);

            services.AddIdentity<User, IdentityRole>()
                .AddDefaultUI()            
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<FishSpinDaysDbContext>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password = new PasswordOptions() // just for easy testing
                {
                    RequiredLength = 4,
                    RequiredUniqueChars = 1,
                    RequireDigit = false,
                    RequireLowercase = true,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false
                };                                
            });

            services.AddAutoMapper(); 

            RegisterServiceLayer(services);

            services
                .AddMvc(options =>
                {
                    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
                })               
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);          
        }

       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
         
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.SeedDatabase();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "area",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        
        private void LoginFromOtherApps(IServiceCollection services)
        {
            services.AddAuthentication()          
           .AddFacebook(options =>
           {
               options.AppId = this.Configuration.GetSection("ExternalAuthentication:Facebook:AppId").Value;
               options.AppSecret = this.Configuration.GetSection("ExternalAuthentication:Facebook:AppSecret").Value;
           })
           .AddGoogle(options =>
           {
               options.ClientId = this.Configuration.GetSection("ExternalAuthentication:Google:ClientId").Value;
               options.ClientSecret = this.Configuration.GetSection("ExternalAuthentication:Google:ClientSecret").Value;
           })
            .AddGitHub(options =>
            {
                options.ClientId = this.Configuration.GetSection("ExternalAuthentication:GitHub:ClientId").Value;
                options.ClientSecret = this.Configuration.GetSection("ExternalAuthentication:GitHub:ClientSecret").Value;
            });
        }

        private static void RegisterServiceLayer(IServiceCollection services)
        {
            services.AddScoped<IAdminSectionsService, AdminSectionsService>();
            services.AddScoped<IBasePublicationsService, BasePublicationsService>();
            services.AddScoped<IAdminPublicationsService, AdminPublicationsService>();      
        }
    }
}
