using CK.Glouton.Lucene;
using CK.Glouton.Model.Services;
using CK.Glouton.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Glouton.Web
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices( IServiceCollection services )
        {
            services.AddOptions();

            services.Configure<LuceneConfiguration>( Configuration.GetSection( "Lucene" ) );
            services.AddSingleton<ILuceneSearcherService, LuceneSearcherService>();
            services.AddSingleton<ILuceneStatisticsService, LuceneStatisticsService>();

            // TODO: Add configuration for alert (especially sender)
            services.AddSingleton<IAlertService, AlertService>();

            services.AddMvc();
        }

        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            if( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();

            app.UseMvc( routes =>
            {
                routes.MapRoute( "default", "{controller=Home}/{action=Index}/{id?}" );
                routes.MapRoute( "spa-fallback", "{*anything}", new { controller = "Home", action = "Index" } );
            } );
        }
    }
}
