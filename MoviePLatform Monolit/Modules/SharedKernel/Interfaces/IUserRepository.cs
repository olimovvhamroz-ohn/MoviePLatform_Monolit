
using MoviePLatform_Monolit.Entity;

public interface IUserRepository : IBaseRepo<UserEntity>
{
    Task<UserEntity> GetByemail(string email);
    Task SetFavoriteCategoriesAsync(long userId, List<long> categoryIds);
    Task<List<string>> GetEmailsByCategoryAsync(long categoryId);
}