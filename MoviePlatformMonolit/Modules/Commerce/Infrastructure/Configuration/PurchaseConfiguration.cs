using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePlatformMonolit.Modules.Users.Domain.Entity;
using MoviePlatformMonolit.Modules.Commerce.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Commerce.Infrastructure.Configuration;

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