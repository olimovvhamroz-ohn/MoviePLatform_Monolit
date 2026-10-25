using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MoviePLatform_Monolit.Modules.Command.UpdateTimestampsInterceptor;

public class UpdateTimestampsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
             UpdateTimestamps(eventData.Context);
        }

        return  base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateTimestamps(DbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified);

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.Metadata.FindProperty("UpdatedAtUtc") != null)
            {
                entry.Property("UpdatedAtUtc").CurrentValue = utcNow;
            }

            if (entry.State == EntityState.Added &&
                entry.Metadata.FindProperty("CreatedAtUtc") != null)
            {
                entry.Property("CreatedAtUtc").CurrentValue = utcNow;
            }
        }
    }
}