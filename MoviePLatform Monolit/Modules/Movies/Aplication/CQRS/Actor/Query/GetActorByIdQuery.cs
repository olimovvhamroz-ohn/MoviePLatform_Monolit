using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Actor.Query;

public record GetByIdActorQuery(int ActorId) : IRequest<ActorResponse>;
public class GetActorByIdQueryHandler:IRequestHandler<GetByIdActorQuery,ActorResponse>
{
    private readonly IActorRepository _actorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActorByIdQueryHandler> _logger;

    public GetActorByIdQueryHandler(IMapper mapper, IActorRepository actorRepository,
        ILogger<GetActorByIdQueryHandler> logger)
    {
        _actorRepository = actorRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ActorResponse> Handle(GetByIdActorQuery request, CancellationToken cancellationToken)
    {
        var actor = await _actorRepository.GetByIdAsync(request.ActorId);
        return _mapper.Map<ActorResponse>(actor);
    }
}