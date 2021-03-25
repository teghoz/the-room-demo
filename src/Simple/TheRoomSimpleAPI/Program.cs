using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TheRoomSimpleAPI.Context;

namespace TheRoomSimpleAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().TheRoomSeedAsync().Result.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
