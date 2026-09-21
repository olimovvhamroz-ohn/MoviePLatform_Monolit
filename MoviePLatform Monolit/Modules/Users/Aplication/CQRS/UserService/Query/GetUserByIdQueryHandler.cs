

using AutoMapper;
using MediatR;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

public record GetUserByIdQuery(int Id) : IRequest<UserResponse>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IMapper mapper, ILogger<GetUserByIdQueryHandler> logger, IUserRepository repository)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by id: {Id}", request.Id);

        var user = await _repository.GetById(request.Id);

        _logger.LogInformation("User found: {Email}", user.Email);

        return _mapper.Map<UserResponse>(user);
    }
}