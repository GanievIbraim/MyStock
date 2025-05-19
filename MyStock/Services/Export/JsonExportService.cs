using MyStock.Services.Export;
using System.Text.Json;
using System.Text;

namespace MyStock.Services.Export
{
    public class JsonExportService : IExportService
    {
        public Task<(Stream, string, string)> ExportAsync<T>(
            IEnumerable<T> entities, ExportFormat format)
        {
            var json = JsonSerializer.Serialize(entities, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var bytes = Encoding.UTF8.GetBytes(json);
            var ms = new MemoryStream(bytes);
            return Task.FromResult<(Stream, string, string)>(
                (ms, "application/json", $"export_{DateTime.UtcNow:yyyyMMddHHmmss}.json"));
        }
    }

}
