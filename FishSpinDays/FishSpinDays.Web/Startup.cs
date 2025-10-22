namespace FishSpinDays.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Web.Common;
    using AutoMapper;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Services.Admin;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Services.Base;
    using System;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Services.Identity;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;
    using FishSpinDays.Web.Hubs;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<FishSpinDaysDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("FishSpinDays"),
                        dbOptions => dbOptions.MigrationsAssembly("FishSpinDays.Data")));

            LoginFromOtherApps(services);

            // only loged in to be able to create posts with API but not using cookies:
            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = "localhost",
                    ValidAudience = "localhost",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(this.Configuration.GetSection("TokenValidationParameter").Value))
                };
            });

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

                options.Lockout.MaxFailedAccessAttempts = 4;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            });

            services.AddAutoMapper();

            // Register the Swagger services
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Fish Spin Days API";
                    document.Info.Description = "ASP.NET Core web API";
                };
            });

            RegisterServiceLayer(services);

            services.AddSignalR();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
            services.AddRazorPages();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // to make Access-Control-Allow-Origin: '*':
            app.UseCors(optins => optins
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // app.UseDeveloperExceptionPage(); // for debugging
                app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
                app.SeedDatabase();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
                app.UseHsts();

                var runMigrations = Configuration.GetValue<bool>("RunMigrationsOnStartup");
                if (runMigrations)
                {
                    using var scope = app.ApplicationServices.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<FishSpinDaysDbContext>();
                    db.Database.Migrate();
                }
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Area route (matches /{area}/{controller}/{action}/{id?})
                endpoints.MapControllerRoute(
                    name: "area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                // Default route (matches /{controller}/{action}/{id?})
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

                endpoints.MapHub<ChatHub>("/chat");
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
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAdminUsersService, AdminUsersService>();
        }
    }
}
