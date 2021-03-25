using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceListAPI;
using System;

namespace ServiceList.FunctionalTests
{
    public class ServiceListTestStartup : Startup
    {
        public ServiceListTestStartup(IConfiguration env) : base(env)
        {
        }
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(Configuration);
            return base.ConfigureServices(services);
        }
        protected override void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration["isTest"] == bool.TrueString.ToLowerInvariant())
            {
                app.UseMiddleware<AutoAuthorizeMiddleware>();
                app.UseAuthorization();
            }
            else
            {
                base.ConfigureAuth(app);
            }
        }
    }
}
