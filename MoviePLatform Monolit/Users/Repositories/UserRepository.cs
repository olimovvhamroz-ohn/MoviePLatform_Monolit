
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Data;
using MoviePLatform_Monolit.Entity;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<UserEntity> GetByemail(string email)
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

    public async Task<List<string>> GetEmailsByCategoryAsync(long categoryId)
    {
        return await _context.UserFavoriteCategories
            .Where(x => x.CategoryId == categoryId && !string.IsNullOrEmpty(x.User.Email))
            .Select(x => x.User.Email)
            .Distinct()
            .ToListAsync();
    }
}