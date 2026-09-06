using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

public record GetMovieByIdQuery(long Id) : IRequest<MovieResponse>;

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieResponse>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMovieByIdQueryHandler> _logger;

    public GetMovieByIdQueryHandler(IMovieRepository repository, IMapper mapper,
        ILogger<GetMovieByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MovieResponse> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting movie by id {MovieId}", request.Id);
        var res = await _repository.GetByIdAsync(request.Id, cancellationToken
        );
        return _mapper.Map<MovieResponse>(res);
    }
}