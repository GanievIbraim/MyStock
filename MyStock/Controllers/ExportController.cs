using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;
using MyStock.Services;
using MyStock.Services.Export;

namespace MyStock.Controllers
{
    [ApiController]
    [Route("api/export/{entity}")]
    public class ExportController : ControllerBase
    {
        private readonly IServiceProvider _provider;
        public ExportController(IServiceProvider provider)
            => _provider = provider;

    [HttpGet]
    public async Task<IActionResult> Export(
        string entity,
        [FromQuery] ExportFormat format = ExportFormat.Json)
    {
        // 1) Явно указываем тип data как IEnumerable<object>
        IEnumerable<object> data = entity.ToLower() switch
        {
            "employees" => (IEnumerable<object>)await _provider
                .GetRequiredService<EmployeeService>()
                .GetAllAsync(),
            "orders"    => (IEnumerable<object>)await _provider
                .GetRequiredService<OrderService>()
                .GetAllAsync(),
            // … другие сущности, тоже кастим к IEnumerable<object>
            _ => throw new ArgumentException($"Unknown entity: {entity}")
        };

        // 2) Выбираем нужный экспортёр
        IExportService exportService = format switch
        {
            ExportFormat.Pdf   => _provider.GetRequiredService<PdfExportService>(),
            ExportFormat.Excel => _provider.GetRequiredService<ExcelExportService>(),
            _                  => _provider.GetRequiredService<JsonExportService>()
        };

        // 3) Генерируем и отдаем файл
        var result = await exportService.ExportAsync(data, format);
        return File(result.Stream, result.ContentType, result.FileName);
    }

    }
}