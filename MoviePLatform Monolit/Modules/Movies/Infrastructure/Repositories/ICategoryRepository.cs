using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

public interface ICategoryRepository : IBaseRepo<CategoryEntity>
{
    Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default);
    
}

public class CategoryRepository : Modules.Movies.Infrastructure.Repositories.BaseRepository<CategoryEntity>, ICategoryRepository
{
    
    
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default)
    {
        var ex = await _context.Categories.AnyAsync(x => x.Title.ToLower() == title.ToLower(),cancellationToken);
        return ex;

    }
}