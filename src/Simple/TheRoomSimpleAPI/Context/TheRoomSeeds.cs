using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheRoomSimpleAPI.Model;

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
        public static async Task<IHost> TheRoomSeedAsync(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                Initialize(services);
                await _context.Database.MigrateAsync();
                await SeedDefaultAdmin(services);
                await SeedServiceList(services);
            }
            return host;
        }

        public static async System.Threading.Tasks.Task SeedServiceList(IServiceProvider provider)
        { 
            using (var context = provider.GetRequiredService<TheRoomContext>())
            {
                context.Database.EnsureCreated();

                var items = new List<ServiceListItem>()
                {
                    new ServiceListItem { Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                    new ServiceListItem { Description = "Oracle things", Name = "oracle.io", Price= 8.50M },
                    new ServiceListItem { Description = "Swagger things", Name = "swagger.io", Price = 12 },
                    new ServiceListItem { Description = "Hangfire Sports", Name = "hangfire.com", Price = 19.5M },
                    new ServiceListItem { Description = "Workkers things", Name = "worker.io", Price= 8.50M },
                    new ServiceListItem { Description = "Test Host things", Name = "testhost.io", Price = 12 },
                    new ServiceListItem { Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                    new ServiceListItem { Description = "Java things", Name = "java.io", Price= 8.50M },
                    new ServiceListItem { Description = "PHP things", Name = "php.com", Price = 12 },
                    new ServiceListItem { Description = "Azure Service", Name = "azure.com", Price = 19.5M },
                    new ServiceListItem { Description = "Kubernetes things", Name = "kubernetes.io", Price= 8.50M },
                    new ServiceListItem { Description = "Razor things", Name = "razor.io", Price = 12 },
                };

                items.ForEach(item => {
                    if(context.tblServiceListItems.Any(s => s.Name == item.Name) == false)
                    {
                        context.tblServiceListItems.Add(item);
                        context.SaveChangesAsync();
                    }
                    
                });
            }  
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
