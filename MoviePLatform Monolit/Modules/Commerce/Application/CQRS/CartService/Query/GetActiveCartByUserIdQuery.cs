using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Query;

public record GetActiveCartByUserId(long UserId) : IRequest<CartResponse>;

public class GetActiveCartByUserIdQuery(ApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetActiveCartByUserId, CartResponse>
{
    public async Task<CartResponse> Handle(GetActiveCartByUserId request, CancellationToken cancellationToken)
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsCheckedOut, cancellationToken);

        if (cart == null)
            return new CartResponse { UserId = request.UserId, IsCheckedOut = false };

        return mapper.Map<CartResponse>(cart);
    }
}