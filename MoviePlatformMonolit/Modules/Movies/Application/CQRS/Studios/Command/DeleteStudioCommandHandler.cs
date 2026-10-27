using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Caching.Distributed;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Studios.Command;

public record DeleteStudioCommand(int Id) : IRequest<StudioResponse>;

public class DeleteStudioCommandHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<DeleteStudioCommandHandler> logger
   //, IDistributedCache cache
    )
    : IRequestHandler<DeleteStudioCommand, StudioResponse>
{
    public async Task<StudioResponse> Handle(DeleteStudioCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new BadRequestException("ID must be greater than zero");

        logger.LogInformation("Deleting studio {Id}", request.Id);

        var studio = await context.Studios
                         .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                     ?? throw new NotFoundException($"Studio with ID {request.Id} was not found");

        var response = mapper.Map<StudioResponse>(studio);

        context.Studios.Remove(studio);
        await context.SaveChangesAsync(cancellationToken);

       /* await cache.RemoveAsync($"studio:{request.Id}", cancellationToken);
        await cache.RemoveAsync("studio:all", cancellationToken);*/

        return response;
    }
}