using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePLatform_Monolit.Entity;

public class CartConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("carts", "commerce");
        builder.HasKey(c => c.Id);

        // Сохтани индекси уникалӣ бо номи сутуни PascalCase дар катича
        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasFilter("\"IsCheckedOut\" = false");
    }
}

public class PurchaseConfiguration : IEntityTypeConfiguration<PurchaseEntity>
{
    public void Configure(EntityTypeBuilder<PurchaseEntity> builder)
    {
        builder.ToTable("purchases", "commerce");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PurchasePrice)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.HasIndex(p => new { p.UserId, p.MovieId })
            .IsUnique();
    }
}