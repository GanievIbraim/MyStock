namespace MyStock.DTO
{
    /// <summary>
    /// Универсальная DTO-ссылка на сущность (Id + отображаемое значение).
    /// Используется, например, в полях типа Organization, Warehouse и т.д.
    /// </summary>
    public class ReferenceDto
    {
        public Guid Id { get; set; }
        public string DisplayValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Представление перечисления или статуса: машинный код и отображаемое имя.
    /// Пример: { code: "terminated", displayValue: "Уволен" }
    /// </summary>
    public class CodeDisplayDto
    {
        public string Code { get; set; } = default!;
        public string DisplayValue { get; set; } = default!;
    }
}
