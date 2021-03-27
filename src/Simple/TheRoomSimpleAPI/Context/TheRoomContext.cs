using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel.DataAnnotations.Schema;
using TheRoomSimpleAPI.Model;
using IdentityUser = Microsoft.AspNetCore.Identity.IdentityUser;

namespace TheRoomSimpleAPI.Context
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
    }

    public class TheRoomContextFactory : IDesignTimeDbContextFactory<TheRoomContext>
    {
        public TheRoomContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TheRoomContext>();
            optionsBuilder.UseSqlServer("Server=tcp:localhost,6433;Database=TheRoom.Service.SimpleDb;User Id =sa;Password=Pass@word;");

            return new TheRoomContext(optionsBuilder.Options);
        }
    }

    public class TheRoomContext : IdentityDbContext<ApplicationUser>
    {
        public TheRoomContext(DbContextOptions<TheRoomContext> options)
        : base(options)
        {
        }

        public DbSet<ServiceListItem> tblServiceListItems { get; set; }
        public DbSet<ServiceListItemPromo> tblServiceListItemPromos { get; set; }
        public DbSet<PromoUsers> tblPromoUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceListItem>().HasData(
                new ServiceListItem { Id = 1, Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                new ServiceListItem { Id = 2, Description = "Oracle things", Name = "oracle.io", Price = 8.50M },
                new ServiceListItem { Id = 3, Description = "Swagger things", Name = "swagger.io", Price = 12 },
                new ServiceListItem { Id = 4, Description = "Hangfire Sports", Name = "hangfire.com", Price = 19.5M },
                new ServiceListItem { Id = 5, Description = "Workkers things", Name = "worker.io", Price = 8.50M },
                new ServiceListItem { Id = 6, Description = "Test Host things", Name = "testhost.io", Price = 12 },
                new ServiceListItem { Id = 7, Description = "Fox Sports", Name = "foxsport.com", Price = 19.5M },
                new ServiceListItem { Id = 8, Description = "Java things", Name = "java.io", Price = 8.50M },
                new ServiceListItem { Id = 9, Description = "PHP things", Name = "php.com", Price = 12 },
                new ServiceListItem { Id = 10, Description = "Azure Service", Name = "azure.com", Price = 19.5M },
                new ServiceListItem { Id = 11, Description = "Kubernetes things", Name = "kubernetes.io", Price = 8.50M },
                new ServiceListItem { Id = 12, Description = "Razor things", Name = "razor.io", Price = 12 }
            );
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseLoggerFactory(GetLoggerFactory())
                //optionsBuilder.UseSqlServer(ConnectionManager.Connection["ConnectionString"]);
            }
        }
    }
}