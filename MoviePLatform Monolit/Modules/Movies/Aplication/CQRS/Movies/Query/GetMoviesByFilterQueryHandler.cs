
using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Entity.Enums;
using MoviePLatform_Monolit.Movie.Repositories;

public record GetMoviesByFilterQuery(
    long? CategoryId, 
    int? Year, 
    decimal? MaxPrice, 
    AgeRating? AgeRating) : IRequest<List<MovieResponse>>;

public class GetMoviesByFilterQueryHandler : IRequestHandler<GetMoviesByFilterQuery, List<MovieResponse>>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;

    public GetMoviesByFilterQueryHandler(IMovieRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<MovieResponse>> Handle(GetMoviesByFilterQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetMovieByFilterAsync(
            request.CategoryId, 
            request.Year, 
            request.MaxPrice, 
            request.AgeRating
        ,cancellationToken);
        return _mapper.Map<List<MovieResponse>>(res);
    }
}