using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Movies.Command;


public record CreateMovieCommand(MovieRequest Dto) : IRequest<MovieResponse>;
public class CreateMovieCommandHandler:IRequestHandler<CreateMovieCommand,MovieResponse>
{
    private readonly IMovieRepository _repo;

    private readonly IMapper _mapper;
    public CreateMovieCommandHandler(IMovieRepository repo, IMapper mapper  )
    {
        _repo = repo; _mapper = mapper; 
    }

    public async Task<MovieResponse> Handle(CreateMovieCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<MovieEntity>(request.Dto);
        var created = await _repo.CreateAsync(entity,ct);


        return _mapper.Map<MovieResponse>(created);
    }
}
