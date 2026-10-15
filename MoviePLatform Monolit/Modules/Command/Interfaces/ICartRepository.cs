using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;

public record CartLine(long MovieId, decimal Price);

public interface ICartRepository
{
    Task<CartEntity?> GetActiveCartByUserId(long userId);
    Task<CartEntity> CreateCart(long userId);
    Task AddMovieToCart(long userId, long movieId);
    Task RemoveMovieFromCart(long userId, long movieId);
    Task<List<CartLine>> GetPayableLines(long userId);

    Task<long> CreatePendingOrder(long userId, IReadOnlyList<CartLine> lines);
    Task CompleteCheckout(long userId, long orderId, IReadOnlyList<CartLine> lines);
    Task MarkOrderFailed(long orderId);
}