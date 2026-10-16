using Microsoft.AspNetCore.Identity;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Command.Interfaces;
using MoviePLatform_Monolit.Modules.Users.Application.CQRS.SmsService;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.Enums;
using MoviePLatform_Monolit.Users.DTO.REQUEST;
using MoviePLatform_Monolit.Users.DTO.RESPONSE;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.AuthService;

public interface IAuthService
{
    Task<LoginResponse> Login(string email, string password);
    Task<LoginResponse> Register(RegisterUserRequest request);
}

public class AuthService(
    IUserRepository repo,
    IJwtService jwt,
    IPasswordHasher<UserEntity> hasher,
    IEmailService emailService) : IAuthService
{
    public async Task<LoginResponse> Register(RegisterUserRequest request)
    {
        var exists = await repo.GetByEmail(request.Email);
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

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        await repo.CreateAsync(user);

        await emailService.SendAsync(
            user.Email,
            "Welcome to MoviePlatform",
            "Your account has been successfully created.");

        var token = jwt.GenerateToken(user);
        return new LoginResponse { Token = token };
    }

    public async Task<LoginResponse> Login(string email, string password)
    {
        var user = await repo.GetByEmail(email);
        if (user == null) throw new NotFoundException("User not Found");

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result != PasswordVerificationResult.Success)
            throw new UnauthorizedException("Wrong password");

        var token = jwt.GenerateToken(user);
        return new LoginResponse { Token = token };
    }
}