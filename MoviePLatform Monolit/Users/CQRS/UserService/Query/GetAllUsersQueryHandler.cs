

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public record GetAllUsersQuery : IRequest<List<UserResponse>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResponse>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IMapper mapper, ILogger<GetAllUsersQueryHandler> logger, IUserRepository repository)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users");

        var users = await _repository.GetAllAsync();

        _logger.LogInformation("Total users: {Count}", users.Count);

        return _mapper.Map<List<UserResponse>>(users);
    }
}