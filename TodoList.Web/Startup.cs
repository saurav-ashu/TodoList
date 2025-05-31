using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using TodoList.Web.Extensions;

namespace TodoList.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Startup>()
                .Build();
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureLocalization();
            services.ConfigureSupportedCultures();
            services.ConfigureCookiePolicy();
            services.ConfigureEntityFramework(Configuration);
            services.ConfigureSecurity();
            services.ConfigureIdentity();
            services.ConfigureStorage(Configuration);
            services.ConfigureSocialAuthentication(Configuration);
            services.ConfigureServices();
            services.ConfigureSendGrid(Configuration);
            services.AddLogging();
            services.AddMvc();
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
                app.UseExceptionHandler("/Home/Error");
                //app.UseHsts();
                //app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            //Add localization supported cultures and request localization via query string (ex: ?culture=en-US)
            var supportedCultures = new CultureInfo[] {
                new CultureInfo("en"),
                new CultureInfo("pt"),
                new CultureInfo("de"),
                new CultureInfo("hi"),
                new CultureInfo("tr"),
                new CultureInfo("fa")

            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();
            app.UseAuthorization(); // Ensure UseAuthentication and UseAuthorization are in the correct order

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
