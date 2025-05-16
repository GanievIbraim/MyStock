using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class ProductCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }

    public class CreateProductCategoryDto
    {
        [Required]
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
    }
}
