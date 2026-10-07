
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<UserEntity?> GetByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task SetFavoriteCategoriesAsync(long userId, List<long> categoryIds)
    {
        var existing = await _context.UserFavoriteCategories
            .Where(x => x.UserId == userId)
            .ToListAsync();

        _context.UserFavoriteCategories.RemoveRange(existing);

        var toAdd = categoryIds.Distinct()
            .Select(categoryId => new UserFavoriteCategoryEntity
            {
                UserId = userId,
                CategoryId = categoryId
            });

        await _context.UserFavoriteCategories.AddRangeAsync(toAdd);
        await _context.SaveChangesAsync();
    }

  
}