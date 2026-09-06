

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record RemoveMovieFromCartCommand(int UserId, int MovieId) : IRequest<CartResponse>;

public class RemoveMovieFromCartCommandHandler(
    ICartRepository cartRepository, 
    ILogger<RemoveMovieFromCartCommandHandler> logger, 
    IMapper mapper) 
    : IRequestHandler<RemoveMovieFromCartCommand, CartResponse>

{
    public async Task<CartResponse> Handle(RemoveMovieFromCartCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing movie {MovieId} from cart for user {UserId}", request.MovieId, request.UserId);

        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var cart = await cartRepository.GetActiveCartByUserId(request.UserId);
        if (cart == null)
            throw new NotFoundException("Cart not found");

        if (cart.CartItems.All(x => x.MovieId != request.MovieId))
            throw new NotFoundException("Movie not found in cart");

        await cartRepository.RemoveMovieFromCart(request.UserId, request.MovieId);
        var updatedCart = await cartRepository.GetActiveCartByUserId(request.UserId);
        return mapper.Map<CartResponse>(updatedCart);
    }
}