

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

public record GetMoviesByNameQuery(string Title) : IRequest<List<MovieResponse>>;

public class GetMoviesByNameQueryHandler : IRequestHandler<GetMoviesByNameQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMoviesByNameQueryHandler> _logger;

    public GetMoviesByNameQueryHandler(IMovieRepository repository, IMapper mapper, ILogger<GetMoviesByNameQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MovieResponse>> Handle(GetMoviesByNameQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching movie by title {Title}", request.Title);
        var res = await _repository.GetMovieByNameAsync(request.Title,cancellationToken);
        return _mapper.Map<List<MovieResponse>>(res);
    }
}