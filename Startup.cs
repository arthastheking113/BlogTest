using BlogTest.Data;
using BlogTest.Models;
using BlogTest.Service;
using BlogTest.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Data
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    DataManage.GetConnectionString(Configuration)));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddTransient<ISlugService, BasicSlugService>();
            services.AddTransient<IImageService, BasicImageService>();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddScoped<IEmailSender, EmailService> ();
            services.AddAuthentication()
                .AddGitHub(options =>
            {
                options.ClientId = "e97d7aee82af0d62c1af";
                options.ClientSecret = "348bfb1766841299913bf5d4049aa4760535da7a";
                options.AccessDeniedPath = "/AccessDeniedPathInfo";
            })
                .AddGoogle(options =>
            {
                options.ClientId = "223021765764-q8m69sfjv45ocr2tasbfc3dgb09m9l4f.apps.googleusercontent.com";
                options.ClientSecret = "4qmzM3PdQnGBhRgP-fi2-HSE";
                options.AccessDeniedPath = "/AccessDeniedPathInfo";
            })
                .AddFacebook(options =>
            {
                options.ClientId = "3621053137942975";
                options.ClientSecret = "658032181e5ef1cfa2f0b5cf22b0c6cb";
                options.AccessDeniedPath = "/AccessDeniedPathInfo";
            })
                 .AddTwitter(options =>
            {
                options.ConsumerKey = "MUeZFzSIM0kiXc9XLNq16QURi";
                options.ConsumerSecret = "1trWiVSuKHSqqxkGJcDh2l7dGJObGOKmx55rAxviOsudPYEyzR";
                options.RetrieveUserDetails = true;
                options.AccessDeniedPath = "/AccessDeniedPathInfo";
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "slugRoute",
                   pattern: "BlogPost/Details/{slug}",
                   defaults: new { Controller = "PostCatagories", action = "Details" });


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}