using MoviePLatform_Monolit.Modules.Commerce.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Command.Interfaces;


public interface IOrderRepository
{
    Task<List<OrderEntity>> GetUserOrders(long userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);
}