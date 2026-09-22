using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MoviePLatform_Monolit.Modules.Movies.Infrastructure.Repositories;
using MoviePLatform_Monolit.Movie.Repositories;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];

    options.InstanceName = "MoviePlatform:";
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.File("tog.txt")
    .Enrich.FromLogContext()
    .CreateLogger();

// 1. Database Context (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repositories & Application Services
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<IStudioRepository, StudioRepository>();
builder.Services.AddScoped<ICategoriyRepository, CategoryRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, Jwtservice>();
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddScoped<PasswordHasher<UserEntity>>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();

builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();
builder.Services.AddScoped<IViewRepository, ViewRepository>();

// 3. MediatR, AutoMapper & FluentValidation
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<Mapping>(), typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 4. JWT Authentication & Authorization
var jwtSecret = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrEmpty(jwtSecret))
    throw new InvalidOperationException("JWT secret key is not configured");

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

// 5. Controllers & OpenAPI (Танҳо як маротиба)
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

// 7. Middlewares
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (!authenticationSchemes.Any(x => x.Name == "Bearer"))
            return;

        // 1. Ба OpenAPI мегӯем, ки Bearer JWT вуҷуд дорад
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT"
                }
            };

        // 2. Bearer-ро ба ҳар endpoint ҳамчун security requirement медиҳем
        foreach (var operation in document.Paths.Values
                     .SelectMany(path => path.Operations))
        {
            operation.Value.Security ??= [];

            operation.Value.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
        }
    }
}