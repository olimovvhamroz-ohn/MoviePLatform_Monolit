using Microsoft.EntityFrameworkCore;
using MoviePLatform_Monolit.Data; // Проверьте, чтобы namespace совпадал с вашей папкой Data

var builder = WebApplication.CreateBuilder(args);

// 1. Регистрация DbContext в контейнере DI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Добавление контроллеров и Swagger/OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. Настройка Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();