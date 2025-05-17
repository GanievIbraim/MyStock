using MyStock.DTO;
using System.ComponentModel.DataAnnotations;

namespace MyStock.Dto {
    public class CreateContactDto
    {
        [Required]
        public string FullName { get; set; } = default!;
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? OrganizationId { get; set; }
    }
    public class ContactDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public ReferenceDto? Organization { get; set; }
    }
};