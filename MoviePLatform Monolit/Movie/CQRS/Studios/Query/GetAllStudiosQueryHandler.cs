using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Query;

public record GetAllStudiosQuery : IRequest<List<StudioResponse>>;

public class GetAllStudiosQueryHandler : IRequestHandler<GetAllStudiosQuery, List<StudioResponse>>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;

    public GetAllStudiosQueryHandler(IStudioRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<StudioResponse>> Handle(GetAllStudiosQuery request, CancellationToken cancellationToken)
    {
        var res = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<StudioResponse>>(res);
    }
}