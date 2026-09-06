using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Movie.DTO.RESPONSE;
using MoviePLatform_Monolit.Movie.Repositories;

namespace MoviePLatform_Monolit.Movie.CQRS.Studios.Command;


public record DeleteStudioCommand(int Id) : IRequest<StudioResponse>;

public class DeleteStudioCommandHandler : IRequestHandler<DeleteStudioCommand, StudioResponse>
{
    private readonly IStudioRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteStudioCommandHandler> _logger;

    public DeleteStudioCommandHandler(IStudioRepository repository, IMapper mapper, ILogger<DeleteStudioCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<StudioResponse> Handle(DeleteStudioCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting studio {Id}", request.Id);

        var deleted = await _repository.DeleteAsync(request.Id,cancellationToken);
        return _mapper.Map<StudioResponse>(deleted);
    }
}