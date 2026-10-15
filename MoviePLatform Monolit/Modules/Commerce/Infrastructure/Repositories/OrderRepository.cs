using MoviePLatform_Monolit.Modules.Command.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    
    public async Task<List<OrderEntity>> GetUserOrders(long userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}