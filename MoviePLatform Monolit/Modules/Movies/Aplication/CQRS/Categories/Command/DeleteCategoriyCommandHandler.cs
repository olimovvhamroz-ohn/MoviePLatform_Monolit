using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Categories.Command;
public record DeleteCategoriy(int id) : IRequest<CategoryResponse>;

public class DeleteCategoriyCommandHandler : IRequestHandler<DeleteCategoriy, CategoryResponse>
{
    private readonly ICateoriyRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteCategoriyCommandHandler> _logger;

    public DeleteCategoriyCommandHandler(ILogger<DeleteCategoriyCommandHandler> logger, ICateoriyRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(DeleteCategoriy request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category with id: {Id}", request.id);
        var deleted = await _repository.DeleteAsync(request.id,cancellationToken);
        _logger.LogInformation("Category deleted with id: {Id}", request.id);
        return _mapper.Map<CategoryResponse>(deleted);
    }
}