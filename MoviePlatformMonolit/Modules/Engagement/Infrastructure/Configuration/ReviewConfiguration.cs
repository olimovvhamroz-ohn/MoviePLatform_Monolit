using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePlatformMonolit.Modules.Engagement.Domain.Entities;

namespace MoviePlatformMonolit.Modules.Engagement.Infrastructure.Configuration;

public class WatchlistConfiguration : IEntityTypeConfiguration<WatchListEntity>
{
    public void Configure(EntityTypeBuilder<WatchListEntity> builder)
    {
        builder.ToTable("watchlist", "engagement");
        builder.HasKey(w => w.Id);

        // Пешгирии дубликат дар Watchlist
        builder.HasIndex(w => new { w.UserId, w.MovieId }).IsUnique();
    }
}