using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePlatformMonolit.Modules.Commerce.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Commerce.Infrastructure.Configuration;

public class CartConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("carts", "commerce");
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasFilter("\"IsCheckedOut\" = false");
    }
}