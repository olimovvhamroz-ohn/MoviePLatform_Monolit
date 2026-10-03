using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

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


