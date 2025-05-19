using System.Text.Json;
using System.Text.Json.Serialization;

using MyStock.Middleware;
using MyStock.Entities;
using MyStock.Services;

using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MyStock.Services.Export;

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

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080); // HTTP
    serverOptions.ListenAnyIP(8443, listenOptions =>
    {
        listenOptions.UseHttps("/etc/ssl/myapp/certificate.pfx", "");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowLocalhost", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();


// Настроим подключение к PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<WarehouseSectionService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<JsonExportService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<PdfExportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("AllowLocalhost"); // CORS
app.UseGlobalExceptionHandler(); // глобальный обработчик ошибок
app.UseAuthorization();
app.MapControllers();

app.Run();
