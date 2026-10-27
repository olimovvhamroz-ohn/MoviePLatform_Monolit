using Microsoft.EntityFrameworkCore.Diagnostics;
using MoviePlatformMonolit.Modules.Command.Data;

namespace MoviePlatformMonolit.Modules.Command.UpdateTimestampsInterceptor;

public class UpdateTimestampsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            EntityAuditor.Apply(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}