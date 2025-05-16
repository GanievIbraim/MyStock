using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class WarehouseSectionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public ReferenceDto Warehouse { get; set; } = default!;
    }

    public class CreateWarehouseSectionDto
    {
        [Required]
        public string Code { get; set; } = default!;
        public string? Description { get; set; }

        [Required]
        public Guid WarehouseId { get; set; }
    }
}
