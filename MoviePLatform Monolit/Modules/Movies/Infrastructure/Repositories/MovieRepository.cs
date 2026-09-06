using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.Entity.Enums;

namespace MoviePLatform_Monolit.Movie.Repositories;

public interface IMovieRepository
{
    Task<List<MovieEntity>> PaginationAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<MovieEntity>> GetMovieByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<MovieEntity>> GetMovieByFilterAsync(long? categoryId, int? year, decimal? maxPrice, AgeRating? ageRating, CancellationToken cancellationToken = default);
    Task<MovieEntity> CreateAsync(MovieEntity movie, CancellationToken cancellationToken = default);
    Task<MovieEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<MovieEntity> UpdateAsync(long id, MovieEntity input, CancellationToken cancellationToken = default);
    Task<MovieEntity> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long movieId, CancellationToken cancellationToken = default);
}

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MovieEntity>> GetMovieByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<MovieEntity>();

        return await _context.Movies
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .Where(x => x.Title.ToLower().Contains(name.ToLower()))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MovieEntity>> PaginationAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var skip = (page - 1) * pageSize;

        return await _context.Movies
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MovieEntity>> GetMovieByFilterAsync(long? categoryId, int? year, decimal? maxPrice, AgeRating? ageRating, CancellationToken cancellationToken = default)
    {
        var query = _context.Movies
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .AsQueryable();

        if (categoryId.HasValue) 
            query = query.Where(x => x.CategoryId == categoryId.Value);

        if (year.HasValue) 
            query = query.Where(x => x.Year == year.Value); // Ислоҳ: Баробарии сол

        if (maxPrice.HasValue) 
            query = query.Where(x => x.Price <= maxPrice.Value);

        if (ageRating.HasValue) 
            query = query.Where(x => x.AgeRating == ageRating.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<MovieEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than zero.", nameof(id));

        var movie = await _context.Movies
            .Include(x => x.Category)
            .Include(x => x.Reviews)
            .Include(x => x.Actors)
            .Include(x => x.Studio)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with ID {id} was not found.");

        return movie;
    }

    public async Task<MovieEntity> CreateAsync(MovieEntity movie, CancellationToken cancellationToken = default)
    {
        if (movie == null) 
            throw new ArgumentNullException(nameof(movie), "Movie entity cannot be null.");

        var isDuplicate = await _context.Movies.AnyAsync(
            x => x.Title.ToLower() == movie.Title.ToLower() && x.Year == movie.Year, 
            cancellationToken);

        if (isDuplicate)
            throw new InvalidOperationException($"Movie with title '{movie.Title}' and year {movie.Year} already exists.");

        var categoryExists = await _context.Categories.AnyAsync(x => x.Id == movie.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with ID {movie.CategoryId} does not exist.");

        await _context.Movies.AddAsync(movie, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return movie;
    }

    public async Task<MovieEntity> UpdateAsync(long id, MovieEntity input, CancellationToken cancellationToken = default)
    {
        if (input == null) 
            throw new ArgumentNullException(nameof(input));

        var entity = await GetByIdAsync(id, cancellationToken);

        entity.Title = input.Title;
        entity.CategoryId = input.CategoryId;
        entity.Year = input.Year;
        entity.Price = input.Price;
        entity.Description = input.Description;
        entity.PosterUrl = input.PosterUrl;
        entity.StudioId = input.StudioId;
        entity.AgeRating = input.AgeRating;

        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<MovieEntity> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var movie = await GetByIdAsync(id, cancellationToken);

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync(cancellationToken);

        return movie;
    }

    public async Task<bool> ExistsAsync(long movieId, CancellationToken cancellationToken = default)
    {
        return await _context.Movies
            .AsNoTracking()
            .AnyAsync(m => m.Id == movieId, cancellationToken);
    }
}