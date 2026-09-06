
using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

public record GetMoviesPaginationQuery(int Page, int PageSize) : IRequest<List<MovieResponse>>;

public class GetMoviesPaginationQueryHandler : IRequestHandler<GetMoviesPaginationQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetMoviesPaginationQueryHandler> _logger;

    public GetMoviesPaginationQueryHandler(IMovieRepository repository, IMapper mapper, ILogger<GetMoviesPaginationQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MovieResponse>> Handle(GetMoviesPaginationQuery request, CancellationToken cancellationToken)
    {
        if (request.Page <= 0 || request.PageSize <= 0)
            throw new Exception(" BadRequestExceptionPage and pageSize must be greater than 0");

        _logger.LogInformation("Getting movies page {Page} size {PageSize}", request.Page, request.PageSize);
        var res = await _repository.PaginationAsync(request.Page, request.PageSize,cancellationToken);
        return _mapper.Map<List<MovieResponse>>(res);
    }
}