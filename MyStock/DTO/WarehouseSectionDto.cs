using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateWarehouseSectionDto
    {
        [Required]
        public string Code { get; set; } = default!;
        [Required]
        public Guid WarehouseId { get; set; }
        
        public string? Description { get; set; }
    }
    public class WarehouseSectionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string? Description { get; set; }

        public ReferenceDto Warehouse { get; set; } = default!;
    }    
}
