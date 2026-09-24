using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Configuration;

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