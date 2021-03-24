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
            optionsBuilder.UseSqlServer("Server=tcp:localhost,6433;Database=TheRoom.Services.TheRoomSimpleDb;User Id =sa;Password=Pass@word;");

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
        public DbSet<PromoUsers> tblServiceProvider { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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