using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Command;

public record DeleteStudioCommand(int Id) : IRequest<StudioResponse>;

public class DeleteStudioCommandHandler(
    IStudioRepository repository,
    IMapper mapper,
    ILogger<DeleteStudioCommandHandler> logger,
    IDistributedCache cache)
    : IRequestHandler<DeleteStudioCommand, StudioResponse>
{
    public async Task<StudioResponse> Handle(
        DeleteStudioCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting studio {Id}", request.Id);

        var deleted = await repository.DeleteAsync(
            request.Id,
            cancellationToken);

        // Remove cache for this studio
        await cache.RemoveAsync(
            $"studio:{request.Id}",
            cancellationToken);

        // Remove cached list of all studios
        await cache.RemoveAsync(
            "studio:all",
            cancellationToken);

        return mapper.Map<StudioResponse>(deleted);
    }
}