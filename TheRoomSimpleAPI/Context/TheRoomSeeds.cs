using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace TheRoomSimpleAPI.Context
{
    public static class TheRoomSeeds
    {
        private static TheRoomContext _context;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        private static IHost _hostingEnvironment;

        public static void Initialize(IServiceProvider provider)
        {
            _context = provider.GetRequiredService<TheRoomContext>();
            _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            _hostingEnvironment = provider.GetRequiredService<IHost>();
        }
        public static async Task<IHost> YoodaloSeedAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Initialize(services);
                await SeedDefaultAdmin(services);
                await _context.Database.MigrateAsync();
            }
            return host;
        }

        public static async System.Threading.Tasks.Task SeedDefaultAdmin(IServiceProvider provider)
        {
            using (var context = provider.GetRequiredService<TheRoomContext>())
            {
                ApplicationUser user = new ApplicationUser
                {
                    FirstName = "Aghogho",
                    LastName = "Bernard",
                    FullName = "Aghogho Bernard",
                    Email = "aghoghobernard@theroom.com",
                    NormalizedEmail = "AGHOGHOBERNARD@THEROOM.COM",
                    PhoneNumber = "+230 5929 1397",
                    UserName = "aghoghobernard@theroom.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                await _userManager.CreateAsync(user, "Pass@word");
            }
        }
    }
}
