using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Movies.Command;


public record UpdateMovieCommand(long Id, MovieRequest Input) : IRequest<MovieResponse>;

public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, MovieResponse>
{
    private readonly IMovieRepository _repository;
    private readonly ICateoriyRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateMovieCommandHandler> _logger;

    public UpdateMovieCommandHandler(
        IMovieRepository repository,
        ICateoriyRepository categoryRepository,
        IMapper mapper,
        ILogger<UpdateMovieCommandHandler> logger )
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MovieResponse> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating movie {Id} via CQRS", request.Id);

        var entity = _mapper.Map<MovieEntity>(request.Input);
        var res = await _repository.UpdateAsync(request.Id, entity, cancellationToken);

        return _mapper.Map<MovieResponse>(res);
    }
}