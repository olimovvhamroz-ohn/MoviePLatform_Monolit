using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.CartService.Command;

public record AddMovieToCart(long UserId, long MovieId) : IRequest<CartResponse>;

public class AddMovieToCartCommand( 
    ICartRepository cartRepository,
    IMovieRepository movie,
    ILogger<AddMovieToCartCommand> logger,
    IMapper mapper) 
    : IRequestHandler<AddMovieToCart, CartResponse>
{
  

    public async Task<CartResponse> Handle(AddMovieToCart request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding movie {MovieId} to cart for user {UserId}", request.MovieId, request.UserId);

        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var movieExists = await movie.ExistsAsync(request.MovieId, cancellationToken);
        if (!movieExists)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var cart = await cartRepository.GetActiveCartByUserId(request.UserId)
                   ?? await cartRepository.CreateCart(request.UserId);

        if (cart.CartItems.Any(x => x.MovieId == request.MovieId))
            throw new BadRequestException("Movie already exists in cart");

        await cartRepository.AddMovieToCart(request.UserId, request.MovieId);
        var result = await cartRepository.GetActiveCartByUserId(request.UserId);
        if (result == null)
            throw new Exception("Failed to retrieve cart after adding movie");
        return mapper.Map<CartResponse>(result);
    }
}