using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Movies.Application.Projections;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Movies.Query;

public record GetMoviesByFilterQuery(
    long? CategoryId,
    int? Year,
    decimal? MaxPrice,
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler(
    ApplicationDbContext context,
    ILogger<GetMoviesByFilterQueryHandler> logger)
    : IRequestHandler<GetMoviesByFilterQuery, List<MovieResponse>>
{
    public async Task<List<MovieResponse>> Handle(
        GetMoviesByFilterQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting movies by filter");

        var query = context.Movies
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (request.CategoryId.HasValue)
            query = query.Where(x => x.CategoryId == request.CategoryId.Value);

        if (request.Year.HasValue)
            query = query.Where(x => x.Year == request.Year.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(x => x.Price <= request.MaxPrice.Value);

        if (request.AgeRating.HasValue)
            query = query.Where(x => x.AgeRating == request.AgeRating.Value);

        return await query
            .OrderBy(x => x.Id)
            .Select(MovieProjections.ToResponse)
            .ToListAsync(cancellationToken);
    }
}