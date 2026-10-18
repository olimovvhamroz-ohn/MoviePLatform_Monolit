using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Command;

public record CreateStudioCommand(StudioRequest Request) : IRequest<StudioResponse>;

public class CreateStudioCommandHandler(
    ApplicationDbContext context,
    IMapper mapper,
    ILogger<CreateStudioCommandHandler> logger)
    : IRequestHandler<CreateStudioCommand, StudioResponse>
{
    public async Task<StudioResponse> Handle(CreateStudioCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Request.Name))
            throw new BadRequestException("Name is required");

        var name = request.Request.Name.Trim();

        logger.LogInformation("Creating studio with name: {Name}", name);

        var exists = await context.Studios
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);

        if (exists)
            throw new BadRequestException("Studio already exists");

        var model = mapper.Map<StudioEntity>(request.Request);
        model.Name = name;

        await context.Studios.AddAsync(model, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Studio created successfully with id: {Id}", model.Id);
        return mapper.Map<StudioResponse>(model);
    }
}