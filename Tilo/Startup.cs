using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tilo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Tilo.Services;

namespace Tilo
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


            services.AddMvc();
            string conString = Configuration["Data:TiloProducts:ConnectionString"];

            services.AddTransient<IProductRepository, EFProductRepository>();
            services.AddTransient<ICategoryRepository, EFCategoryRepository>();
            services.AddTransient<IFileModelRepository, EFFileModelRepository>();



            services.AddTransient<ProductsService>();
            services.AddTransient<PhotosService>();
            services.AddTransient<EmailService>();
            services.AddTransient<IOrdersRepository, EFOrdersRepository>();
            services.AddDistributedMemoryCache();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(conString)
            );


            services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(Configuration["Data:TiloIdentity:ConnectionString"]));
            services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

            services.AddDistributedSqlServerCache(options => {
                options.ConnectionString = conString;
                options.SchemaName = "dbo";
                options.TableName = "SessionData";
            });
            services.AddSession(options => {
                options.Cookie.Name = "Tilo.Session";
                options.IdleTimeout = System.TimeSpan.FromHours(48);
                options.Cookie.HttpOnly = false;
            });
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Use(async (ctx, next) => {
                        ctx.Request.Path = "/Home/Error";
                        await next();
                    });
                    appBuilder.UseMvc(routes => {
                        routes.MapRoute(
                            name: "sth-wrong",
                            template: "{controller=Home}/{action=Error}");
                    });
                });
            }

            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {

                //routes.MapRoute(
                //    name: "List",
                //    template: "Shop/{action}",
                //    defaults: new {controller = "Shop", action = "List" }
                //    );

                //routes.MapRoute(
                //    name: "Product",
                //    template: "Product/{id}",
                //    defaults: new { controller = "Shop", action = "Product"  }
                //    );
                routes.MapRoute(
                    name: "admin",
                   template: "Admin/{action}/{productId}"
                    );
                //routes.MapRoute(
                //   name: "categoryNavAdmin",
                //   template: "{category}/{page}",
                //   defaults: new { controller = "Admin", action = "Index" }
                //);
                //routes.MapRoute(
                //    name: "Shop",
                //    template: "Shop",
                //    defaults: new { controller = "Shop", action = "Index" }
                //    );
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            });
            //SeedData.EnsurePopulated(app);
            //IdentitySeedData.EnsurePopulated(app).Wait();
        }
    }
}
