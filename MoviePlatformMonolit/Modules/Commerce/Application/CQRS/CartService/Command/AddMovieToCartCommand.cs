using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Commerce.Domain.Entity;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Commerce.Application.CQRS.CartService.Command;

public record AddMovieToCart(long UserId, long MovieId) : IRequest<CartResponse>;

public class AddMovieToCartCommand(
    ApplicationDbContext context,
    ILogger<AddMovieToCartCommand> logger,
    IMapper mapper) : IRequestHandler<AddMovieToCart, CartResponse>
{
    public async Task<CartResponse> Handle(AddMovieToCart request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        logger.LogInformation("Adding movie {MovieId} to cart for user {UserId}", request.MovieId, request.UserId);

        var movieExists = await context.Movies
            .AnyAsync(x => x.Id == request.MovieId, cancellationToken);

        if (!movieExists)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var cart = await context.Carts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && !x.IsCheckedOut, cancellationToken);

        if (cart == null)
        {
            cart = new CartEntity { UserId = request.UserId };
            context.Carts.Add(cart);
        }

        if (cart.CartItems.Any(x => x.MovieId == request.MovieId))
            throw new BadRequestException("Movie already exists in cart");

        cart.CartItems.Add(new CartItemEntity { MovieId = request.MovieId });
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<CartResponse>(cart);
    }
}