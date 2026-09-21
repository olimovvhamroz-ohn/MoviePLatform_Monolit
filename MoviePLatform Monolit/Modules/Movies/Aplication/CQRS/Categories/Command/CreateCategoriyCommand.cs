using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Categories.Command;
public record CreateCategoryCommand(CategoryRequest Request):IRequest<CategoryResponse>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly ICategoriyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCategoryCommand> _logger;

    public CreateCategoryCommandHandler(ILogger<CreateCategoryCommand> logger, ICategoriyRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category with title: {Title}", request.Request.Title);


        var exit = await _repository.ExistsByNameAsync(request.Request.Title,cancellationToken);
        if (exit)
        {
            _logger.LogWarning("Category already exists: {Title}", request.Request.Title);
            throw new Exception(" BadRequestExceptioncategory already exists");
        }

        var model = _mapper.Map<CategoryEntity>(request.Request);
        var created = await _repository.CreateAsync(model,cancellationToken);

        _logger.LogInformation("Category created successfully with id: {Id}", created.Id);
        return _mapper.Map<CategoryResponse>(created);
    }
}