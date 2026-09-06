

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.Repositories;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record AddMovieToCart(int userId, int movieId) : IRequest<CartResponse>;

public class AddMovieToCartCommand : IRequestHandler<AddMovieToCart, CartResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMovieRepository _movie; // ✅ вместо AppDbcontext
    private readonly IMapper _mapper;
    private readonly ILogger<AddMovieToCartCommand> _logger;

    public AddMovieToCartCommand(
        ICartRepository cartRepository,
        IMovieRepository movieReadModel,
        ILogger<AddMovieToCartCommand> logger,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _movie = movieReadModel;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CartResponse> Handle(AddMovieToCart request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding movie {MovieId} to cart for user {UserId}", request.movieId, request.userId);

        if (request.userId <= 0 || request.movieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var movieExists = await _movie.ExistsAsync(request.movieId, cancellationToken);
        if (!movieExists)
            throw new NotFoundException($"Movie {request.movieId} not found");

        var cart = await _cartRepository.GetActiveCartByUserId(request.userId);
        if (cart == null)
            throw new NotFoundException("Cart not found");

        if (cart.CartItems.Any(x => x.MovieId == request.movieId))
            throw new BadRequestException("Movie already exists in cart");

        await _cartRepository.AddMovieToCart(request.userId, request.movieId);
        var result = await _cartRepository.GetActiveCartByUserId(request.userId);
        return _mapper.Map<CartResponse>(result);
    }
}