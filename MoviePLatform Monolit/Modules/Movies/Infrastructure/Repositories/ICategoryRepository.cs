using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

public interface ICategoryRepository : IBaseRepo<CategoryEntity>
{
    Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default);
    
}

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<CategoryEntity>(context), ICategoryRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> ExistsByNameAsync(string title, CancellationToken cancellationToken = default)
    {
        var ex = await _context.Categories.AnyAsync(x => x.Title.ToLower() == title.ToLower(),cancellationToken);
        return ex;

    }
}