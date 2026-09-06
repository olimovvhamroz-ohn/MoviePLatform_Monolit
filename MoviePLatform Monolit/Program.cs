using System.Text;
using EngagementService.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MoviePLatform_Monolit.Entity;
using MoviePLatform_Monolit.Movie.Repositories;
using MoviePLatform_Monolit.Users.CQRS.AuthService;


var builder = WebApplication.CreateBuilder(args);

// 1. Database Context (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositories & Application Services
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IViewRepository, ViewRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService,Jwtservice>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

// 3. MediatR, AutoMapper & FluentValidation
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Ислоҳ барои AutoMapper:
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 4. JWT Authentication & Authorization
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? "SuperSecretKey12345678901234567890";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// 5. Controllers & OpenApi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// 6. Custom Exception Middleware (Пеш аз ҳама миддлверҳо бояд бошад)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 7. Auth Middleware Order
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();