using MoviePLatform_Monolit.Modules.SharedKernel.Interfaces;

namespace MoviePLatform_Monolit.Modules.Commerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderEntity>> GetUserOrders(long userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}