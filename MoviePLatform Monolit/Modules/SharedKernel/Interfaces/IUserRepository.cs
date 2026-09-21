
public interface IUserRepository : IBaseRepo<UserEntity>
{
    Task<UserEntity> GetByEmail(string email);
    Task SetFavoriteCategoriesAsync(long userId, List<long> categoryIds);
    Task<List<string>> GetEmailsByCategoryAsync(long categoryId);
}