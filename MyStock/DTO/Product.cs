using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public ProductUnit Unit { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? SupplierId { get; set; }
    }
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = default!;

        [Required]
        public string Code { get; set; } = default!;

        public string? Barcode { get; set; }
        public string? Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal Price { get; set; }

        public ProductUnit Unit { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public Guid? SectionId { get; set; }
        public Guid? SupplierId { get; set; }
    }

}
