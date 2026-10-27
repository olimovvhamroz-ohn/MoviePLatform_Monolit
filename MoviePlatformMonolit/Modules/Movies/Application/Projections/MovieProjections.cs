using System.Linq.Expressions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Movies.Domain.Entity;

namespace MoviePlatformMonolit.Modules.Movies.Application.Projections;

public static class MovieProjections
{
    public static readonly Expression<Func<MovieEntity, MovieResponse>> ToResponse =
        movie => new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Price = movie.Price,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            Duration = movie.Duration,
            AgeRating = movie.AgeRating,

            Category = movie.Category == null
                ? null
                : new CategoryResponse
                {
                    Id = movie.Category.Id,
                    Title = movie.Category.Title
                },

            Studio = movie.Studio == null
                ? null
                : new StudioResponse
                {
                    Id = movie.Studio.Id,
                    Name = movie.Studio.Name,
                    Country = movie.Studio.Country,
                    FoundYear = movie.Studio.FoundYear
                },

            Actors = movie.Actors
                .Where(a => !a.IsDeleted)
                .Select(a => new ActorResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Biography = a.Biography
                })
                .ToList(),

            Reviews = movie.Reviews
                .Where(r => !r.IsDeleted)
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToList()
        };
}