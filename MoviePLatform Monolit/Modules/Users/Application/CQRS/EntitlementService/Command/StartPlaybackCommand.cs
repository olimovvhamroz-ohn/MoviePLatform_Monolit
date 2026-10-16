using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.EntitlementService.Command;

public record StartPlaybackCommand(long UserId, long MovieId) : IRequest<MovieAccessResultResponse>;

public class StartPlaybackCommandHandler(
    IUserRepository userRepository,
    IMovieRepository movieRepository,
    IPurchaseRepository purchaseRepository) : IRequestHandler<StartPlaybackCommand, MovieAccessResultResponse>
{
    public async Task<MovieAccessResultResponse> Handle(StartPlaybackCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId <= 0 || request.MovieId <= 0)
            throw new BadRequestException("Invalid userId or movieId");

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException($"User {request.UserId} not found");

        var movie = await movieRepository.GetByIdAsync(request.MovieId, cancellationToken);
        if (movie == null)
            throw new NotFoundException($"Movie {request.MovieId} not found");

        var purchase = await purchaseRepository.GetByUserAndMovie(request.UserId, request.MovieId);

        var now = DateTime.UtcNow;
        var check = EntitlementEvaluator.CheckAccess(user, movie, purchase, now);

        if (!check.CanWatch)
            return check;

        if (purchase!.FirstWatchedAt == null)
        {
            purchase.FirstWatchedAt = now;
            await purchaseRepository.UpdatePurchase(purchase);
        }

        return EntitlementEvaluator.CheckAccess(user, movie, purchase, now);
    }
}