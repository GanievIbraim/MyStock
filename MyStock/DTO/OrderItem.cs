using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateOrderItemDto
    {
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        public Guid? SectionId { get; set; }
    }

    public class OrderItemDto
    {
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public Guid OrderId { get; set; }
        public string OrderName { get; set; } = default!;
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; } = default!;
    }
}
