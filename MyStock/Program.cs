using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MyStock.Middleware;
using MyStock.Entities;
using MyStock.Services;

var rawEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var envFile = $".env.{rawEnv.ToLower()}";
if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{rawEnv}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();

// Настроим подключение к PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<WarehouseSectionService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<UserService>();

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
