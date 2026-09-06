using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Categories.Query;
public record GetAllCategories: IRequest<List<CategoryResponse>>;

public class GetAllCatecoriyQuery : IRequestHandler<GetAllCategories, List<CategoryResponse>>
{
    private readonly ICateoriyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllCatecoriyQuery> _logger;

    public GetAllCatecoriyQuery(
        ILogger<GetAllCatecoriyQuery> logger, 
        ICateoriyRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<CategoryResponse>> Handle(GetAllCategories request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all categories");        
        var data = await _repository.GetAllAsync(cancellationToken);
        
        _logger.LogInformation("Total categories found: {Count}", data.Count);
        
        return _mapper.Map<List<CategoryResponse>>(data);
    }
}