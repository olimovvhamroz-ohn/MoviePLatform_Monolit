
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

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