using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Query;

public record GetActorByNameQuery(string Name) : IRequest<ActorResponse>;

public class GetActorByNameQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetActorByNameQuery, ActorResponse>
{
    public async Task<ActorResponse> Handle(GetActorByNameQuery request, CancellationToken cancellationToken)
    {
        var name = request.Name?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(name))
            throw new BadRequestException("Name is required");

        return await context.Actors
                   .AsNoTracking()
                   .Where(x => !x.IsDeleted && x.Name.ToLower() == name)
                   .Select(x => new ActorResponse
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Biography = x.Biography
                   })
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new NotFoundException($"Actor {request.Name} not found");
    }
}