using Microsoft.EntityFrameworkCore;
using MyStock.Middleware;
using MyStock.Entities;
using MyStock.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Настроим подключение к PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var serviceType = typeof(IService);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
var types = assemblies
    .SelectMany(x => x.GetTypes())
    .Where(t => serviceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

foreach (var type in types)
{
    builder.Services.AddScoped(type);
}

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler(); // добавляем глобальный обработчик ошибок
app.UseAuthorization();
app.MapControllers();

app.Run();
