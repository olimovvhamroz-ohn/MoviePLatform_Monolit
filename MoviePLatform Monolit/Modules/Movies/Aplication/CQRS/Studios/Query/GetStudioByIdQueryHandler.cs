using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Query;


    
public record GetStudioByIdQuery(int Id) : IRequest<StudioResponse>;

public class GetStudioByIdQueryHandler : IRequestHandler<GetStudioByIdQuery, StudioResponse>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;

    public GetStudioByIdQueryHandler(IStudioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<StudioResponse> Handle(GetStudioByIdQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetByIdAsync(request.Id,cancellationToken);
        return _mapper.Map<StudioResponse>(res);
    }
}