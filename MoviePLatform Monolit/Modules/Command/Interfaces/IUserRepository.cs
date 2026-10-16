

using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public interface IUserRepository : IBaseRepo<UserEntity>
{
    Task<UserEntity?> GetByEmail(string email);
    Task SetFavoriteCategoriesAsync(long userId, List<long> categoryIds);
}