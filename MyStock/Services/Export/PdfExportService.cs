using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MyStock.Services.Export
{
    public class PdfExportService : IExportService
    {
        public Task<(Stream Stream, string ContentType, string FileName)>
            ExportAsync<T>(System.Collections.Generic.IEnumerable<T> entities, ExportFormat format)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            // Приводим к списку, чтобы можно было взять первый элемент
            var list = entities.Cast<object>().ToList();

            // Определяем тип элементов:
            // если T == object, пытаемся взять реальный тип первого элемента
            var elementType = typeof(T);
            if (elementType == typeof(object) && list.Any())
                elementType = list.First().GetType();

            // Получаем все публичные свойства этого типа
            var props = elementType.GetProperties();
            if (props.Length == 0)
                throw new InvalidOperationException(
                    $"Невозможно создать PDF: тип {elementType.Name} не содержит публичных свойств для колонок");

            // Генерируем PDF
            var ms = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                        .Text($"Экспорт {elementType.Name}")
                        .FontSize(20).SemiBold().AlignCenter();

                    page.Content()
                        .Table(table =>
                        {
                            // 1) Определяем колонки
                            table.ColumnsDefinition(cols =>
                            {
                                foreach (var _ in props)
                                    cols.RelativeColumn();
                            });

                            // 2) Заголовки
                            table.Header(header =>
                            {
                                foreach (var prop in props)
                                    header.Cell().Text(prop.Name).SemiBold();
                            });

                            // 3) Данные
                            foreach (var item in list)
                            {
                                foreach (var prop in props)
                                {
                                    var text = prop.GetValue(item)?.ToString() ?? "";
                                    table.Cell().Text(text);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x => x.CurrentPageNumber());
                });
            })
            .GeneratePdf(ms);

            ms.Position = 0;
            var fileName = $"export_{elementType.Name}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var contentType = "application/pdf";

            return Task.FromResult<(Stream, string, string)>((ms, contentType, fileName));
        }
    }
}