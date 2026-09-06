

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;

public record CreateUserCommand(UserRequest Dto) : IRequest<UserResponse>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly PasswordHasher<UserEntity> _hasher;

    public CreateUserCommandHandler(
        PasswordHasher<UserEntity> hasher,
        IMapper mapper,
        IUserRepository repository,
        ILogger<CreateUserCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _hasher = hasher;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        _logger.LogInformation("Creating user with email: {Email}", dto.Email);

        var exists = await _repository.GetByemail(dto.Email);
        if (exists != null)
            throw new BadRequestException("User already exists");

        var model = _mapper.Map<UserEntity>(dto);
        model.PasswordHash = _hasher.HashPassword(model, dto.Password);

        var created = await _repository.CreateAsync(model);

        _logger.LogInformation("User created with id: {Id}", created.Id);
        return _mapper.Map<UserResponse>(created);
    }
}