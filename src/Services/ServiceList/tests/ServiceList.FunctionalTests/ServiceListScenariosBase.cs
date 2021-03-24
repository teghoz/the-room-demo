using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceListAPI;
using ServiceListAPI.Extensions;
using ServiceListAPI.Infrastructure;
using System.IO;
using System.Reflection;
using TheRoom.BuildingBlocks.IntegrationEventLogEF;

namespace ServiceList.FunctionalTests
{
    public class ServiceListScenariosBase
    {
        public TestServer CreateServer()
        {
            var path = Assembly.GetAssembly(typeof(ServiceListScenariosBase))
              .Location;

            var hostBuilder = new WebHostBuilder()
                .UseContentRoot(Path.GetDirectoryName(path))
                .ConfigureAppConfiguration(cb =>
                {
                    cb.AddJsonFile("appsettings.json", optional: false)
                    .AddEnvironmentVariables();
                })
                .UseStartup<ServiceListTestStartup>();

            var testServer = new TestServer(hostBuilder);

            testServer.Host
                .MigrateDbContext<ServiceListContext>((context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var settings = services.GetService<IOptions<ServiceListSettings>>();
                    var logger = services.GetService<ILogger<ServiceListContextSeed>>();
                    new ServiceListContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
                })
                .MigrateDbContext<IntegrationEventLogContext>((_, __) => { });

            return testServer;
        }

        public static class Get
        {
            private const int PageIndex = 0;
            private const int PageCount = 4;

            public static string Items(bool paginated = false)
            {
                return paginated
                    ? "api/v1/ServiceList/items" + Paginated(PageIndex, PageCount)
                    : "api/v1/ServiceList/items";
            }

            public static string ItemById(int id)
            {
                return $"api/v1/ServiceList/items/{id}";
            }

            public static string ItemByName(string name, bool paginated = false)
            {
                return paginated
                    ? $"api/v1/ServiceList/items/withname/{name}" + Paginated(PageIndex, PageCount)
                    : $"api/v1/ServiceList/items/withname/{name}";
            }

            private static string Paginated(int pageIndex, int pageCount)
            {
                return $"?pageIndex={pageIndex}&pageSize={pageCount}";
            }
        }
    }
}
