using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Data;
using MoviePLatform_Monolit.Entity;

namespace MoviePLatform_Monolit.Movie.Repositories;

public interface ICateoriyRepository : IBaseRepo<CategoryEntity>
{
    Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default);
    
}

public class CateoryRepository : BaseRepository<CategoryEntity>, ICateoriyRepository
{
    
    
    public CateoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default)
    {
        var ex = await _context.Categories.AnyAsync(x => x.Title.ToLower() == title.ToLower(),cancellationToken);
        return ex;

    }
}