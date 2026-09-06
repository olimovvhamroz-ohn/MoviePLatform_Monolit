using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Movies.Command;

public record DeleteMovieCommand(long Id) : IRequest<MovieResponse>;

public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, MovieResponse>
{
    private readonly IMovieRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteMovieCommandHandler> _logger;


    public DeleteMovieCommandHandler(
        IMovieRepository repository,
        IMapper mapper,
        ILogger<DeleteMovieCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MovieResponse> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting movie {Id} via CQRS", request.Id);

        var res = await _repository.DeleteAsync(request.Id,cancellationToken);


        return _mapper.Map<MovieResponse>(res);
    }
}