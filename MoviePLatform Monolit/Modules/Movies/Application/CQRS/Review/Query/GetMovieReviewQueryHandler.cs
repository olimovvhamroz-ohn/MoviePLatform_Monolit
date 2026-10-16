using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Review.Query;

public record GetMovieReviewsQuery(long MovieId, int Page, int PageSize)
    : IRequest<List<ReviewResponse>>;

public class GetMovieReviewQueryHandler(IReviewRepository repository, IMapper mapper)
    : IRequestHandler<GetMovieReviewsQuery, List<ReviewResponse>>
{
    public async Task<List<ReviewResponse>> Handle(GetMovieReviewsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
        {
            throw new BadRequestException(
                "Page and pageSize must be greater than 0");
        }

        var requestSend = await repository.GetMovieReviews(request.MovieId, request.Page, request.PageSize);
        return mapper.Map<List<ReviewResponse>>(requestSend);
    }
}