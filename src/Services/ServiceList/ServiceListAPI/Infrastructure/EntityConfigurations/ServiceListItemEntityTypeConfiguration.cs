using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceListAPI.Model;

namespace ServiceListAPI.Infrastructure.EntityConfigurations
{
    class ServiceListItemEntityTypeConfiguration
        : IEntityTypeConfiguration<ServiceListItem>
    {
        public void Configure(EntityTypeBuilder<ServiceListItem> builder)
        {
            builder.ToTable("ServiceLists");

            builder.Property(ci => ci.Id)
                .UseHiLo("service_list_hilo")
                .IsRequired();

            builder.Property(ci => ci.Description)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Name)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Price)
                .IsRequired(true);
        }
    }
}
