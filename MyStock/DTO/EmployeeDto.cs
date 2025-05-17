using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateEmployeeDto
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        public string? Patronymic { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Guid? WarehouseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ContactId { get; set; }
    }

    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Patronymic { get; set; }
        public string? Position { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public CodeDisplayDto Status { get; set; } = default!;
        public ReferenceDto? Warehouse { get; set; }
        public ReferenceDto? Organization { get; set; }
        public ReferenceDto? Contact { get; set; }
    }
}
