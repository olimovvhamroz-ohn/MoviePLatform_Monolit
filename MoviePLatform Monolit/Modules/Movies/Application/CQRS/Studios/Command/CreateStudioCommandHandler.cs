using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Movies.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Movies.Domain.Entity;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;

namespace MoviePLatform_Monolit.Modules.Movies.Application.CQRS.Studios.Command;

public record CreateStudioCommand(StudioRequest request) : IRequest<StudioResponse>;

public class CreateStudioCommandHandler(
    IStudioRepository repository,
    IMapper mapper,
    ILogger<CreateStudioCommandHandler> logger)
    : IRequestHandler<CreateStudioCommand, StudioResponse>
{
    public async Task<StudioResponse> Handle(CreateStudioCommand req, CancellationToken cancellationToken)
    {
        var dto = req.request;
        logger.LogInformation("Creating studio with name: {Name}", dto.Name);

        var all = await repository.GetAllAsync(cancellationToken);
        if (all.Any(x => x.Name.ToLower() == dto.Name.ToLower()))
            throw new Exception(" BadRequestExceptionStudio already exists");

        var model = mapper.Map<StudioEntity>(dto);
        var created = await repository.CreateAsync(model, cancellationToken);

        logger.LogInformation("Studio created successfully with id: {Id}", created.Id);
        return mapper.Map<StudioResponse>(created);
    }
}