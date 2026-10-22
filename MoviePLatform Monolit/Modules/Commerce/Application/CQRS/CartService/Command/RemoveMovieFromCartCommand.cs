using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Commerce.Application.CQRS.CartService.Command;

public record RemoveMovieFromCartCommand(long UserId, long MovieId) : IRequest<CartResponse>;

public class RemoveMovieFromCartCommandHandler(
    ApplicationDbContext context,
    ILogger<RemoveMovieFromCartCommandHandler> logger,
    IMapper mapper) : IRequestHandler<RemoveMovieFromCartCommand, CartResponse>
{
    public async Task<CartResponse> Handle(RemoveMovieFromCartCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        logger.LogInformation("Removing movie {MovieId} from cart for user {UserId}", request.MovieId, request.UserId);

        var cart = await context.Carts
                       .Include(x => x.CartItems)
                       .FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsCheckedOut, cancellationToken)
                   ?? throw new NotFoundException("Cart not found");

        var item = cart.CartItems.FirstOrDefault(x => x.MovieId == request.MovieId)
                   ?? throw new NotFoundException("Movie not found in cart");

        cart.CartItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<CartResponse>(cart);
    }
}