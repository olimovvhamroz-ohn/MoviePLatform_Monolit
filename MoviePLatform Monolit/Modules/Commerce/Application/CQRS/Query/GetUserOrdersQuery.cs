using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Commerce.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.Query;

public record GetUserOrdersQuery(long UserId, int PageNumber = 1, int PageSize = 10)
    : IRequest<List<OrderResponse>>;

public class GetUserOrdersQueryHandler(IOrderRepository repository) : IRequestHandler<GetUserOrdersQuery, List<OrderResponse>>
{
    private const int MaxPageSize = 50;
    
    public async Task<List<OrderResponse>> Handle(GetUserOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var orders = await repository.GetUserOrders(
            request.UserId, pageNumber, pageSize, cancellationToken);

        return orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            UserId = o.UserId,
            OrderDate = o.OrderDate,
            Status = o.Status.ToString(),
            TotalPrice = o.TotalPrice,
            Items = o.Items.Select(i => new OrderItemResponse
            {
                MovieId = i.MovieId,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        }).ToList();
    }
}