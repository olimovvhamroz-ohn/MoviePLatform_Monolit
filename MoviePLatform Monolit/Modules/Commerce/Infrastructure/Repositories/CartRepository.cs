using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Enum;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Repositories;

public class CartRepository(ApplicationDbContext context) : ICartRepository
{
   

    public async Task<CartEntity?> GetActiveCartByUserId(long userId)
    {
        return await context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);
    }

    public async Task<CartEntity> CreateCart(long userId)
    {
        var cart = new CartEntity { UserId = userId };
        
        await context.Carts.AddAsync(cart);
        
        await context.SaveChangesAsync();
        
        return cart;
    }
    
    public async Task AddMovieToCart(long userId, long movieId)
        
        
    {
        var movieExists = await context.Movies.AnyAsync(x => x.Id == movieId);
        
        if (!movieExists) throw new NotFoundException($"Movie with id {movieId} not found");

        var cart = await context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);

        if (cart == null)
            throw new NotFoundException("Active cart not found");

        cart.CartItems.Add(new CartItemEntity()
        {
            CartId = cart.Id,
            MovieId = movieId
        });

        await context.SaveChangesAsync();
    }

    public async Task RemoveMovieFromCart(long userId, long movieId)
    {
        var cart = await context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsCheckedOut);
        
        if (cart == null)
            throw new NotFoundException("Active cart not found");
        
        var cartItem = cart.CartItems.FirstOrDefault(x => x.MovieId == movieId);
       
        if (cartItem == null)
            throw new NotFoundException("Movie not found in cart");

        cart.CartItems.Remove(cartItem);
        await context.SaveChangesAsync();
    }

    public async Task<List<CartLine>> GetPayableLines(long userId)
    {
        var cart = await  context.Carts
            .AsNoTracking()
            .Include(x => x.CartItems)
            .Where(x => x.UserId == userId && !x.IsCheckedOut)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        if (cart == null) throw new NotFoundException("Active cart not found");

        var movieIds = cart.CartItems.Select(i => i.MovieId).Distinct().ToList();

        var alreadyBought = await context.Purchases
            .Where(x => x.UserId == userId && movieIds.Contains(x.MovieId))
            .Select(x => x.MovieId)
            .ToListAsync();

        return await context.Movies
            .AsNoTracking()
            .Where(m => movieIds.Contains(m.Id) && !alreadyBought.Contains(m.Id))
            .Select(m => new CartLine(m.Id, m.Price))
            .ToListAsync();
    }

    // Қадами 1: Order ва Payment бо ҳолати Pending (ҳанӯз пул гирифта нашудааст)
    public async Task<long> CreatePendingOrder(long userId, IReadOnlyList<CartLine> lines)
    {
        var total = lines.Sum(l => l.Price);

        var order = new OrderEntity
        {
            UserId = userId,
            TotalPrice = total,
            Status = OrderStatus.Pending,
            Items = lines.Select(l => new OrderItemEntity
            {
                MovieId = l.MovieId,
                Price = l.Price,
                Quantity = 1
            }).ToList(),
            Payment = new PaymentEntity
            {
                Amount = total,
                Status = PaymentStatus.Pending,
                PaymentMethod = PaymentMethod.CreditCard // баъдтар аз дархости корбар гирифта мешавад
            }
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
        return order.Id;
    }

    // Қадами 2а: пардохт муваффақ шуд → Completed + Purchase + корзина баста мешавад (як транзаксия)
    public async Task CompleteCheckout(long userId, long orderId, IReadOnlyList<CartLine> lines)
    {
        var order = await context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        if (order == null) throw new NotFoundException("Order not found");

        var cart = await context.Carts
            .Include(x => x.CartItems)
            .Where(x => x.UserId == userId && !x.IsCheckedOut)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
        if (cart == null) throw new NotFoundException("Active cart not found");

        foreach (var line in lines)
        {
            await context.Purchases.AddAsync(new PurchaseEntity
            {
                UserId = userId,
                MovieId = line.MovieId,
                PurchasePrice = line.Price,
                PurchasedAt = DateTime.UtcNow
            });
        }

        order.Status = OrderStatus.Completed;
        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.Completed;
            order.Payment.ProcessedAt = DateTime.UtcNow;
        }

        cart.IsCheckedOut = true;
        cart.CartItems.Clear();

        await context.SaveChangesAsync();
    }

    // Қадами 2б: пардохт нагузашт → Cancelled / Failed
    public async Task MarkOrderFailed(long orderId)
    {
        var order = await context.Orders
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return;

        order.Status = OrderStatus.Cancelled;
        if (order.Payment != null)
        {
            order.Payment.Status = PaymentStatus.Failed;
            order.Payment.ProcessedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}