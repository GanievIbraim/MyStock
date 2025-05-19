using ClosedXML.Excel;

namespace MyStock.Services.Export
{
    public class ExcelExportService : IExportService
    {
        public Task<(Stream, string, string)> ExportAsync<T>(
            IEnumerable<T> entities, ExportFormat format)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");
            // Автоматический вывод заголовков на основе свойств DTO
            ws.Cell(1, 1).InsertTable(entities);
            var ms = new MemoryStream();
            wb.SaveAs(ms);
            ms.Position = 0;
            return Task.FromResult<(Stream, string, string)>(
                (ms,
                 "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 $"export_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx"));
        }
    }
}
