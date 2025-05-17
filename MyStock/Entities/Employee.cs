using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.Json;
using MyStock.Json;

namespace MyStock.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Patronymic { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public DateTime? DateOfBirth { get; set; }

        public Guid? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }

        public Guid? ContactId { get; set; }
        public Contact? Contact { get; set; }

        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }

    /// <summary>
    /// Статусы сотрудника.
    /// Сериализуется в JSON как строка в camelCase.
    /// </summary>
    [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
    public enum EmployeeStatus
    {
        [Display(Name = "Активен")]
        Active = 0,

        [Display(Name = "Приостановлен")]
        Suspended = 1,

        [Display(Name = "Уволен")]
        Terminated = 2,

        [Display(Name = "В отпуске")]
        OnLeave = 3
    }
}
