using MediatR;
using MovieService.Infrastructure.Repositories;
using UserService.Application.CQRS.EntitlementService;
using UserService.Application.DTO.RESPONSE;
using UserService.Domain.Interfaces;

namespace UserService.Application.CQRS.EntitlementService.Query;

public record CheckEntitlementQuery(long UserId, long MovieId) : IRequest<EntitlementResponse>;

public class CheckEntitlementQueryHandler : IRequestHandler<CheckEntitlementQuery, EntitlementResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IPurchaseRepository _purchaseRepository;

    public CheckEntitlementQueryHandler(
        IUserRepository userRepository,
        IMovieRepository movieRepository,
        IPurchaseRepository purchaseRepository)
    {
        _userRepository = userRepository;
        _movieRepository = movieRepository;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<EntitlementResponse> Handle(CheckEntitlementQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var user = await _userRepository.GetById(request.UserId);

        var movie = await _movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var purchase = await _purchaseRepository.GetByUserAndMovie(request.UserId, request.MovieId);

        return EntitlementEvaluator.Evaluate(user, movie, purchase, DateTime.UtcNow);
    }
}