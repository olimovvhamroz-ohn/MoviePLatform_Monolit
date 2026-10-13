using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.DTO.REQUEST;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Command;
public record CreateStudioCommand(StudioRequest request) : IRequest<StudioResponse>;

public class CreateStudioCommandHandler : IRequestHandler<CreateStudioCommand, StudioResponse>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateStudioCommandHandler> _logger;

    public CreateStudioCommandHandler(IStudioRepository repository, IMapper mapper, ILogger<CreateStudioCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<StudioResponse> Handle(CreateStudioCommand req, CancellationToken cancellationToken)
    {
        var dto = req.request;
        _logger.LogInformation("Creating studio with name: {Name}", dto.Name);

        var all = await _repository.GetAllAsync(cancellationToken);
        if (all.Any(x => x.Name.ToLower() == dto.Name.ToLower()))
            throw new Exception(" BadRequestExceptionStudio already exists");

        var model = _mapper.Map<StudioEntity>(dto);
        var created = await _repository.CreateAsync(model,cancellationToken);

        _logger.LogInformation("Studio created successfully with id: {Id}", created.Id);
        return _mapper.Map<StudioResponse>(created);
    }
}