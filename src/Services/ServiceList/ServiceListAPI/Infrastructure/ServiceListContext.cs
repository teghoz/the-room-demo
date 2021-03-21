namespace ServiceListAPI.Infrastructure
{
    using EntityConfigurations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using ServiceListAPI.Model;

    public class ServiceListContext : DbContext
    {
        public ServiceListContext(DbContextOptions<ServiceListContext> options) : base(options)
        {
        }
        public DbSet<ServiceListItem> ServiceListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ServiceListItemEntityTypeConfiguration());
        }
    }

    public class ServiceListContextDesignFactory : IDesignTimeDbContextFactory<ServiceListContext>
    {
        public ServiceListContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ServiceListContext>()
                .UseSqlServer("Server=.;Initial Catalog=TheRoom.Services.ServiceListDb;Integrated Security=true");

            return new ServiceListContext(optionsBuilder.Options);
        }
    }
}
