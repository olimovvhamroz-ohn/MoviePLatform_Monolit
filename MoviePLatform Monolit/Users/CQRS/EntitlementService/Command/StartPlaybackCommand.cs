
using MediatR;
using MoviePLatform_Monolit.Movie.Repositories;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Application.CQRS.EntitlementService;
using UserService.Domain.Extensions;

public record StartPlaybackCommand(long UserId, long MovieId) : IRequest<EntitlementResponse>;

public class StartPlaybackCommandHandler : IRequestHandler<StartPlaybackCommand, EntitlementResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IPurchaseRepository _purchaseRepository;

    public StartPlaybackCommandHandler(
        IUserRepository userRepository,
        IMovieRepository movieRepository,
        IPurchaseRepository purchaseRepository)
    {
        _userRepository = userRepository;
        _movieRepository = movieRepository;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<EntitlementResponse> Handle(StartPlaybackCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var user = await _userRepository.GetById(request.UserId);

        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var purchase = await _purchaseRepository.GetByUserAndMovie(request.UserId, request.MovieId);

        var now = DateTime.UtcNow;
        var check = EntitlementEvaluator.Evaluate(user, movie, purchase, now);

        if (!check.CanWatch)
            return check;

        if (purchase!.FirstWatchedAt == null)
        {
            purchase.FirstWatchedAt = now;
            await _purchaseRepository.UpdatePurchase(purchase);
        }

        return EntitlementEvaluator.Evaluate(user, movie, purchase, now);
    }
}