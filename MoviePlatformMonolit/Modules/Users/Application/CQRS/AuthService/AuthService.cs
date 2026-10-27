using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviePlatformMonolit.Modules.Command.Data;
using MoviePlatformMonolit.Modules.Command.Extensions;
using MoviePlatformMonolit.Modules.Users.Application.CQRS.Notifications;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.REQUEST;
using MoviePlatformMonolit.Modules.Users.Domain.DTO.RESPONSE;
using MoviePlatformMonolit.Modules.Users.Domain.Entity;
using MoviePlatformMonolit.Modules.Users.Domain.Enums;

namespace MoviePlatformMonolit.Modules.Users.Application.CQRS.AuthService;

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

        if (await context.Users.AnyAsync(x => x.Email == email))
            throw new BadRequestException("User already exists");

        var user = new UserEntity
        {
            Email = email,
            Phone = request.Phone,
            Name = request.Name,
            Role = Role.Client,
            DateOfBirth = NormalizeDateOfBirth(request.DateOfBirth)
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

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == normalized && !x.IsDeleted);

        if (user is null)
            throw new UnauthorizedException("Invalid email or password");

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password");

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, password);
            await context.SaveChangesAsync();
        }

        return new LoginResponse { Token = jwt.GenerateToken(user) };
    }

    public async Task<LoginResponse> RefreshToken(long userId)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted)
            ?? throw new UnauthorizedException("User no longer exists");

        return new LoginResponse { Token = jwt.GenerateToken(user) };
    }

    private static DateTime? NormalizeDateOfBirth(DateTime? value)
    {
        if (value is null)
            return null;

        var date = value.Value.Date;

        if (date > DateTime.UtcNow.Date)
            throw new BadRequestException("Date of birth cannot be in the future");

        return DateTime.SpecifyKind(date, DateTimeKind.Utc);
    }
}