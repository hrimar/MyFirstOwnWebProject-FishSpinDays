namespace FishSpinDays.Web
{
    using AutoMapper;
    using FishSpinDays.Data;
    using FishSpinDays.Models;
    using FishSpinDays.Services.Admin;
    using FishSpinDays.Services.Admin.Interfaces;
    using FishSpinDays.Services.Base;
    using FishSpinDays.Services.Base.Interfaces;
    using FishSpinDays.Services.Identity;
    using FishSpinDays.Services.Identity.Interfaces;
    using FishSpinDays.Web.Common;
    using FishSpinDays.Web.Configuration;
    using FishSpinDays.Web.Hubs;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Buffers.Text;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

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

            // Configure JWT Settings
            var jwtSettings = new JwtSettings();
            Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);

            // Get existing TokenValidationParameter (for backward compatibility)
            var existingTokenKey = Configuration.GetSection("TokenValidationParameter").Value;

            LoginFromOtherApps(services);

            // JWT Bearer for APIs - using existing TokenValidationParameter as fallback
            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                options.SaveToken = jwtSettings.SaveToken;

                // Use JWT Key if configured, otherwise fallback to existing TokenValidationParameter
                var signingKey = !string.IsNullOrEmpty(jwtSettings.Key) ? jwtSettings.Key : existingTokenKey;

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    
                    ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkew)
                };

                // event handlers for better logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogWarning("JWT Authentication failed: {Exception}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogDebug("JWT Token validated for user: {UserId}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

            // Default Cookie-based Identity authentication for web pages
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
