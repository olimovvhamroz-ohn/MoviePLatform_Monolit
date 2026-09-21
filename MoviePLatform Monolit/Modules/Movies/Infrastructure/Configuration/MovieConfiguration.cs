using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

public class MovieConfiguration : IEntityTypeConfiguration<MovieEntity>
{
    public void Configure(EntityTypeBuilder<MovieEntity> builder)
    {
        builder.ToTable("movies", "catalog");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title).IsRequired().HasMaxLength(300);
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Price).HasColumnType("numeric(10,2)").IsRequired();

        builder.HasMany(m => m.Reviews)
            .WithOne()
            .HasForeignKey(r => r.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Единственная и правильная настройка связи Многие-ко-Многим
        builder.HasMany(m => m.Actors)
            .WithMany(a => a.Movies)
            .UsingEntity(
                "movie_actors",
                l => l.HasOne(typeof(ActorEntity)).WithMany().HasForeignKey("actor_id"),
                r => r.HasOne(typeof(MovieEntity)).WithMany().HasForeignKey("movie_id"),
                j => j.ToTable("movie_actors", "catalog"));
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> builder)
    {
        builder.ToTable("reviews", "catalog");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.Comment).HasMaxLength(1000);

        builder.HasIndex(r => new { r.MovieId, r.UserId }).IsUnique();
    }
}