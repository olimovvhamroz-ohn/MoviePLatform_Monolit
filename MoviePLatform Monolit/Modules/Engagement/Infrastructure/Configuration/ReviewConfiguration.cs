using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviePLatform_Monolit.Modules.Engagement.Domain.Entities;

namespace MoviePLatform_Monolit.Modules.Engagement.Infrastructure.Configuration;

public class WatchlistConfiguration : IEntityTypeConfiguration<WatchlistEntity>
{
    public void Configure(EntityTypeBuilder<WatchlistEntity> builder)
    {
        builder.ToTable("watchlist", "engagement");
        builder.HasKey(w => w.Id);

        // Пешгирии дубликат дар Watchlist
        builder.HasIndex(w => new { w.UserId, w.MovieId }).IsUnique();
    }
}