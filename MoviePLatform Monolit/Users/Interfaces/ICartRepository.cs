using MoviePLatform_Monolit.Entity;

public interface ICartRepository
{
    Task<CartEntity> GetActiveCartByUserId(int userId);
    Task<CartEntity> CreateCart(long userId);
    Task AddMovieToCart(int userId, int movieId);
    Task RemoveMovieFromCart(int userId, int movieId);
    Task CheckoutCart(int userId);
}