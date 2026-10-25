using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Modules.Command.Data;
using MoviePLatform_Monolit.Modules.Command.Extensions;
using MoviePLatform_Monolit.Modules.Users.Application.CQRS.Notifications;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.REQUEST;
using MoviePLatform_Monolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePLatform_Monolit.Modules.Users.Domain.Entity;
using MoviePLatform_Monolit.Modules.Users.Domain.Enums;

namespace MoviePLatform_Monolit.Modules.Users.Application.CQRS.AuthService;

public interface IAuthService
{
    Task<LoginResponse> Login(string email, string password);
    Task<LoginResponse> Register(RegisterUserRequest request);
    Task<LoginResponse> RefreshToken(long userId);
}

public class AuthService(
    ApplicationDbContext context,
    IJwtService jwt,
    IPasswordHasher<UserEntity> hasher,
    IEmailService emailService) : IAuthService
{
    public async Task<LoginResponse> Register(RegisterUserRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await context.Users.AnyAsync(x => x.Email == email);
        if (exists)
            throw new BadRequestException("User already exists");

        var user = new UserEntity
        {
            Email = email,
            Phone = request.Phone,
            Name = request.Name,
            Role = Role.Client,
            DateOfBirth = request.DateOfBirth
        };

        user.PasswordHash = hasher.HashPassword(user, request.Password);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await emailService.SendAsync(
            user.Email,
            "Welcome to MoviePlatform",
            "Your account has been successfully created.");

        return new LoginResponse { Token = jwt.GenerateToken(user) };
    }

    public async Task<LoginResponse> Login(string email, string password)
    {
        var normalized = email.Trim().ToLowerInvariant();

        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == normalized);

        // Одно сообщение для «нет пользователя» и «неверный пароль»
        if (user == null)
            throw new UnauthorizedException("Invalid email or password");

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password");

        return new LoginResponse { Token = jwt.GenerateToken(user) };
    }
    public async Task<LoginResponse> RefreshToken(long userId)
    {
        var user = await context.Users
                       .AsNoTracking()
                       .FirstOrDefaultAsync(x => x.Id == userId)
                   ?? throw new UnauthorizedException("User no longer exists");

        return new LoginResponse { Token = jwt.GenerateToken(user) };
    }
}