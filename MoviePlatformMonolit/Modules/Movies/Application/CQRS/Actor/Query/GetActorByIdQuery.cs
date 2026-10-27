using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Movies.Domain.DTO.RESPONSE;

namespace MoviePlatformMonolit.Modules.Movies.Application.CQRS.Actor.Query;

public record GetByIdActorQuery(int ActorId) : IRequest<ActorResponse>;

public class GetActorByIdQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetByIdActorQuery, ActorResponse>
{
    public async Task<ActorResponse> Handle(GetByIdActorQuery request, CancellationToken cancellationToken)
    {
        if (request.ActorId <= 0)
            throw new BadRequestException("ID must be greater than zero");

        return await context.Actors
                   .AsNoTracking()
                   .Where(x => !x.IsDeleted && x.Id == request.ActorId)
                   .Select(x => new ActorResponse
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Biography = x.Biography
                   })
                   .FirstOrDefaultAsync(cancellationToken)
               ?? throw new NotFoundException($"Actor with id {request.ActorId} not found");
    }
}