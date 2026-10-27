using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Interfaces;

namespace MoviePlatformMonolit.Modules.Command.Data;

public static class EntityAuditor
{
    public static void Apply(DbContext context)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var clrType = entry.Metadata.ClrType;

            if (entry.State == EntityState.Added &&
                typeof(IHasCreation).IsAssignableFrom(clrType))
            {
                entry.Property(nameof(IHasCreation.CreatedAtUtc)).CurrentValue = utcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified &&
                typeof(IHasUpdate).IsAssignableFrom(clrType))
            {
                entry.Property(nameof(IHasUpdate.UpdatedAtUtc)).CurrentValue = utcNow;
            }
        }
    }
}