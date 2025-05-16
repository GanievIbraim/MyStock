using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class OrganizationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? LegalForm { get; set; }
        public string? INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public OrganizationType Type { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }
    public class CreateOrganizationDto
    {
        [Required]
        public string Name { get; set; } = default!;

        public string? LegalForm { get; set; }
        public string? INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        [Required]
        public OrganizationType Type { get; set; }
        public Guid? PrimaryContactId { get; set; }
    }

}
