using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Categories.Query;


public record GetCategoryByIdQuery(int Id) : IRequest<CategoryResponse>;


public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly ICateoriyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        ILogger<GetCategoryByIdQueryHandler> logger, 
        ICateoriyRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting category by id: {Id} via CQRS", request.Id);
        
        var data = await _repository.GetByIdAsync(request.Id,cancellationToken);
        if (data == null)
        {
            _logger.LogWarning("Category not found with id: {Id}", request.Id);
            return null;
        }

        _logger.LogInformation("Category found: {Title}", data.Title);

        return _mapper.Map<CategoryResponse>(data);
    }
}