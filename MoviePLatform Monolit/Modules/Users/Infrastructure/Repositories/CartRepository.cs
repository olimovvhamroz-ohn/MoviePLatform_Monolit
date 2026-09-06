

using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Entity;
using UserService.Domain.Extensions;

public class CartRepository : ICartRepository
{
    protected ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext con)
    {
        _context = con;
    }

    public async Task<CartEntity?> GetActiveCartByUserId(int userId)
    {
        return await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);
    }

    public async Task<CartEntity> CreateCart(long userId)
    {
        var cart = new CartEntity { UserId = userId };
        await _context.Carts.AddAsync(cart);
        //todo : izmenit karan darkor
        await _context.SaveChangesAsync();
        return cart;
    }

    public async Task AddMovieToCart(int userId, int movieId)
    {
        var movieExists = await _context.Movies.AnyAsync(x => x.Id == movieId);
        {
            if (!movieExists) throw new NotFoundException($"$Movie with id {movieId} not found");
        }
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);

        if (cart == null)
            throw new NotFoundException("Active cart not found");
        cart.CartItems.Add(new CartItemEntity()
        {
            CartId = cart.Id,
            MovieId = movieId
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveMovieFromCart(int userId, int movieId)
    {
        var cart = await _context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);

        var cartItem = cart.CartItems.FirstOrDefault(x => x.MovieId == movieId);
        cart.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }

    public async Task CheckoutCart(int userId)
    {var cart = await _context.Carts
            .Include(x => x.CartItems)
            .Where(x => x.UserId == userId && !x.IsCheckedOut)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        if (cart == null) throw new NotFoundException("Active cart not found");
        foreach (var item in cart.CartItems)
        {
            var alreadyBought = await _context.Purchases
                .AnyAsync(x => x.UserId == userId && x.MovieId == item.MovieId);

            if (!alreadyBought)
            {
                await _context.Purchases.AddAsync(new PurchaseEntity()
                {
                    UserId = userId,
                    MovieId = item.MovieId,
                    PurchasedAt = DateTime.UtcNow
                });
            }
        }

        cart.IsCheckedOut = true;
        cart.CartItems.Clear();
        await _context.SaveChangesAsync();
    }
}