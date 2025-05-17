using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Code { get; set; } = default!;

        [Required]
        public Guid CategoryId { get; set; }

        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public Guid? SectionId { get; set; }
        public Guid? SupplierId { get; set; }

        public ProductUnit Unit { get; set; }
    }
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public CodeDisplayDto Unit { get; set; } = default!;
        public ReferenceDto? Category { get; set; }
        public ReferenceDto? Section { get; set; }
        public ReferenceDto? Supplier { get; set; }
    }
}
