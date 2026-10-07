using Microsoft.AspNetCore.Identity;
using MoviePLatform_Monolit.Enums;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;
using UserService.Domain.Extensions;


public interface IAuthService
{
    Task<LoginResponse> Login(string email, string password);
    Task<LoginResponse> Register(RegisterUserRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _repo;
    private readonly IJwtService _jwt;
    private readonly IPasswordHasher<UserEntity> _hasher;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository repo, IJwtService jwt,
        IPasswordHasher<UserEntity> hasher,
        IEmailService emailService)
    {
        _repo = repo; 
        _jwt = jwt;
        _hasher = hasher;
        _emailService = emailService;

    }

    public async Task<LoginResponse> Register(RegisterUserRequest request)
    {
        var exists = await _repo.GetByEmail(request.Email);
        if (exists != null)
            throw new BadRequestException("User already exists");

        var user = new UserEntity
        {
            Email = request.Email,
            Phone = request.Phone,
            Name = request.Name,
            Role = Role.Client,
            DateOfBirth = request.DateOfBirth
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        await _repo.CreateAsync(user);
        
        await _emailService.SendAsync(
            user.Email,
            "Welcome to MoviePlatform",
            "Your account has been successfully created.");

        var token = _jwt.GenerateToken(user);
        return new LoginResponse { Token = token };
    }

    public async Task<LoginResponse> Login(string email, string password)
    {
        var user = await _repo.GetByEmail(email);
        if (user == null) throw new NotFoundException("User not Found");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result != PasswordVerificationResult.Success)
            throw new UnauthorizedException("Wrong password");

        var token = _jwt.GenerateToken(user);
        return new LoginResponse { Token = token };
    }
}