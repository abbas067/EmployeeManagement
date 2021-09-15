using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Employee_Management.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
namespace Employee_Management
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection")));
            services.AddIdentity<ApplicationUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
            // configuring password rules using password options 
            services.Configure<IdentityOptions>(options=>
                {
                    options.Password.RequiredLength = 10;
                    //overrides password required length from 6 to 10
                    options.Password.RequiredUniqueChars = 3;
                    //overrides required unique chars to 3 from 1.
            });
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            //Localization code start
            services.AddControllersWithViews();
            services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Resources";
            });
            services.Configure<RequestLocalizationOptions>(
          opts =>
          {
              var supportedCultures = new List<CultureInfo>
              {     new CultureInfo("en-US"),
                    new CultureInfo("en-GB"),//UK English
                    new CultureInfo("zh-CN"),//Simplified Chinese
                    new CultureInfo("es-US"),//Latin American Spanish
                    new CultureInfo("fi-FI"),//Finnish
                    new CultureInfo("fr-CA"),//Canadian French
                    new CultureInfo("nl-BE"),//Dutch
                    new CultureInfo("pl"),//Polish
                    new CultureInfo("pt-BR"),//Brazilian Portuguese
                    new CultureInfo("sv-SE")//Swedish
              };

              opts.DefaultRequestCulture = new RequestCulture("en-US");
              // Formatting numbers, dates, etc.
              opts.SupportedCultures = supportedCultures;
              // UI strings that we have localized.
              opts.SupportedUICultures = supportedCultures;
          });
            services.AddMvc().AddViewLocalization();// to register localization
            //Localization code end   
            // adding claim based authorization
            services.AddAuthorization(Options =>
                {
                    Options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("DeleteClaim"));
                });
            // added external login provider google
            services.AddAuthentication().AddGoogle(
                Options =>
                {
                    Options.ClientId = "235553987769-due6ua43i96rnfe063ls708apf2t2ktb.apps.googleusercontent.com";
                    Options.ClientSecret = "PTUKomK62DPbJg7PjqsarJHT";
                })
                .AddFacebook(
                Options =>
                {
                    Options.ClientId = "376783153835811";
                    Options.ClientSecret = "a745168c08ddc222290cdcbe9deb3486";
                })

                ;

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error/{0}");
            }
            //localization

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            //localization ends
            app.UseStaticFiles();
            app.UseAuthentication();

             //app.UseMvcWithDefaultRoute();
             app.UseMvc(routes => {
               routes.MapRoute("default","{controller=Home}/{action=Index}/{id?}");
             });

        }
    }
}
