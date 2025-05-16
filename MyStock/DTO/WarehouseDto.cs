using System.ComponentModel.DataAnnotations;
using MyStock.Extensions;

namespace MyStock.DTO
{
    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public ReferenceDto? Owner { get; set; }
    }

    public class CreateWarehouseDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
