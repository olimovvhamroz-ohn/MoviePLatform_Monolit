using MediatR;
using AutoMapper;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public record UpdateFavoriteGenresCommand(long UserId, UpdateFavoriteGenresRequest Dto) : IRequest<UserResponse>;

public class UpdateFavoriteGenresCommandHandler : IRequestHandler<UpdateFavoriteGenresCommand, UserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateFavoriteGenresCommandHandler> _logger;

    public UpdateFavoriteGenresCommandHandler(IUserRepository repository, IMapper mapper, ILogger<UpdateFavoriteGenresCommandHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(UpdateFavoriteGenresCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId,cancellationToken); // бросит NotFoundException, если нет

        await _repository.SetFavoriteCategoriesAsync(request.UserId, request.Dto.CategoryIds);

        _logger.LogInformation("User {UserId} updated favorite genres: {Ids}", request.UserId, string.Join(",", request.Dto.CategoryIds));

        var updated = await _repository.GetByIdAsync(request.UserId,cancellationToken);
        return _mapper.Map<UserResponse>(updated);
    }
}