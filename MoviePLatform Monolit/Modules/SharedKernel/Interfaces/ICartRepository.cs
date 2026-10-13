using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

public record CartLine(long MovieId, decimal Price);

public interface ICartRepository
{
    Task<CartEntity?> GetActiveCartByUserId(int userId);
    Task<CartEntity> CreateCart(long userId);
    Task AddMovieToCart(int userId, int movieId);
    Task RemoveMovieFromCart(int userId, int movieId);
    Task<List<CartLine>> GetPayableLines(int userId);

    Task<long> CreatePendingOrder(int userId, IReadOnlyList<CartLine> lines);
    Task CompleteCheckout(int userId, long orderId, IReadOnlyList<CartLine> lines);
    Task MarkOrderFailed(long orderId);
}