using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Review.Query;

public record GetTopMoviesQuery(int Page,int PageSize) : IRequest<List<MovieResponse>>;

public class GetTopMoviesQueryHandler : IRequestHandler<GetTopMoviesQuery, List<MovieResponse>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTopMoviesQueryHandler> _logger;

    public GetTopMoviesQueryHandler(IReviewRepository reviewRepository, IMapper mapper,ILogger<GetTopMoviesQueryHandler> logger)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MovieResponse>> Handle(GetTopMoviesQuery request, CancellationToken cancellationToken)
    {
        if (request.Page<=0 || request.PageSize<=0)
            throw new ("BadRequestExceptionPage and pageSize must be greater than 0");
        _logger.LogInformation("Getting top movies page {Page} size {PageSize}", request.Page, request.PageSize);
        var movies = await _reviewRepository.GetTopMovie(request.Page,request.PageSize);
        return _mapper.Map<List<MovieResponse>>(movies);
    }
}