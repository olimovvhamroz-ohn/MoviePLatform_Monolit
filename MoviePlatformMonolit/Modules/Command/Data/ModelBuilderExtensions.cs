using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Interfaces;

namespace MoviePlatformMonolit.Modules.Command.Data;

public static class ModelBuilderExtensions
{
    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(IHasDeletion).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeleted = Expression.Property(parameter, nameof(IHasDeletion.IsDeleted));
            var filter = Expression.Lambda(Expression.Not(isDeleted), parameter);

            entityType.SetQueryFilter(filter);
        }
    }
}