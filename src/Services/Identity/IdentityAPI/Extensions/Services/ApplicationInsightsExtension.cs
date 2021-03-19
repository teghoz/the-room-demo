using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityAPI.Extensions.Service
{
    public static class ApplicationInsightsExtensions
    {
        public static IServiceCollection RegisterApplicationInsights (this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddApplicationInsightsTelemetry(configuration);
        }
    }
}
