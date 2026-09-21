using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Command;

public record DeleteStudioCommand(int Id) : IRequest<StudioResponse>;

public class DeleteStudioCommandHandler : IRequestHandler<DeleteStudioCommand, StudioResponse>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteStudioCommandHandler> _logger;
    private readonly IDistributedCache _cache;

    public DeleteStudioCommandHandler(
        IStudioRepository repository,
        IMapper mapper,
        ILogger<DeleteStudioCommandHandler> logger,
        IDistributedCache cache)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<StudioResponse> Handle(
        DeleteStudioCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting studio {Id}", request.Id);

        var deleted = await _repository.DeleteAsync(
            request.Id,
            cancellationToken);

        // Remove cache for this studio
        await _cache.RemoveAsync(
            $"studio:{request.Id}",
            cancellationToken);

        // Remove cached list of all studios
        await _cache.RemoveAsync(
            "studio:all",
            cancellationToken);

        return _mapper.Map<StudioResponse>(deleted);
    }
}