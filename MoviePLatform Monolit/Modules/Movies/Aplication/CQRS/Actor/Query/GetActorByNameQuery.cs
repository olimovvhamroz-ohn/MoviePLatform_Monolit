using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Query;

public record GetActoName(string name) : IRequest<ActorResponse>;

public class GetActorByNameQuery : IRequestHandler<GetActoName, ActorResponse>
{
    private readonly IActorRepository _actorRepository;
    private readonly IMapper _mapper;

    public GetActorByNameQuery(IActorRepository actorRepository, IMapper mapper)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
    }

    
    public async Task<ActorResponse> Handle(GetActoName request, CancellationToken cancellationToken)
    {
        var actor = await _actorRepository.GetByName(request.name);
        return _mapper.Map<ActorResponse>(actor);
    }

}
