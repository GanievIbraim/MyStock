namespace MyStock.Services.Export
{
    public interface IExportService
    {
        /// <summary>
        /// Возвращает готовый файл в указанном формате.
        /// </summary>
        Task<(Stream Stream, string ContentType, string FileName)> ExportAsync<T>(
            IEnumerable<T> entities, ExportFormat format);
    }

    public enum ExportFormat { 
        Pdf, 
        Excel, 
        Json
    }
}
