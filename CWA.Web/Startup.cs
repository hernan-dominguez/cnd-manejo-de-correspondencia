using CWA.AccessControl;
using CWA.AccessControl.Extensions;
using CWA.Application;
using CWA.Data;
using CWA.Entities.Identity;
using CWA.Models.Comun;
using CWA.Shared.Helpers;
using CWA.Web.Settings;
using CWA.Web.Settings.GrandesClientes;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config) => _config = config;

        // Add (and configure) services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Global application, and other settings
            services.Configure<ApplicationSettings>(_config);

            // Application
            services.AddApplicationLayerServices();
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(180));

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.PageViewLocationFormats.Add("/Pages/Shared/Partials/{0}" + RazorViewEngine.ViewExtension);
                options.PageViewLocationFormats.Add("/Pages/GrandesClientes/Partials/{0}" + RazorViewEngine.ViewExtension);
                options.PageViewLocationFormats.Add("/Pages/Protecciones/Partials/{0}" + RazorViewEngine.ViewExtension);
                options.PageViewLocationFormats.Add("/Pages/ViabilidadContratos/Partials/{0}" + RazorViewEngine.ViewExtension);
                options.PageViewLocationFormats.Add("/Pages/ManejoDeCorrespondencia/Partials/{0}" + RazorViewEngine.ViewExtension);
            });

            // Data store
            services.AddDbContext<DataContext>(options => options.UseOracle(_config["Database:ConnectionString"]));

            // Security
            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<DataContext>();

            services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = _config["Authentication:Microsoft:ClientId"];
                options.ClientSecret = _config["Authentication:Microsoft:ClientSecret"];
                options.AuthorizationEndpoint = _config["Authentication:Microsoft:AuthorizationEndpoint"];
                options.TokenEndpoint = _config["Authentication:Microsoft:TokenEndpoint"];

                options.CallbackPath = "/signin-ark";
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = ".cndwebapps.app.cookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.LoginPath = "/SignIn";

                // Prevent redirection to default view for unauthorized responses
                options.Events.OnRedirectToAccessDenied = new Func<RedirectContext<CookieAuthenticationOptions>, Task>(context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                });
            });

            services.AddAuthorization(options => options.AddPolicies());
            services.AddAccessControlLayerServices();
            services.Configure<RazorPagesOptions>(options => options.AddFoldersAuthorization());
            services.Configure<AntiforgeryOptions>(options => options.Cookie.Name = ".cndwebapps.af.cookie");
        }

        // Configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Error handling
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Error handling
                app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                    {
                        var pack = new ViewPack($"{context.Response.StatusCode}", "Hemos tenido problemas para procesar su solicitud.");
                        var htmlBody = await RazorTemplateEngine.RenderAsync("~/Pages/Shared/Partials/_ErrorPagePartial.cshtml", pack);
                        await context.Response.WriteAsync(htmlBody);
                    });
                });

                // Enforce strict https
                app.UseHsts();
            }

            app.UseStatusCodePages(async context =>
            {
                // Custom response for 400/500 errors
                var pack = new ViewPack($"{context.HttpContext.Response.StatusCode}", "No se puede procesar su solicitud ya que la misma no es válida o el recurso indicado no existe.");
                var htmlBody = await RazorTemplateEngine.RenderAsync("~/Pages/Shared/Partials/_ErrorPagePartial.cshtml", pack);
                await context.HttpContext.Response.WriteAsync(htmlBody);
            });

            // Navigation
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Security
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoints
            app.UseEndpoints(endpoints => endpoints.MapRazorPages());
        }
    }
}