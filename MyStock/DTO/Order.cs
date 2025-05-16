using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = default!;
        public DateTime? ApprovedAt { get; set; }

        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }

        public Guid WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        public Guid? ContactId { get; set; }
        public string? ContactName { get; set; }

        public string? Comment { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderDto
    {
        [Required]
        public string Number { get; set; } = default!;

        [Required]
        public OrderType Type { get; set; }

        public string? Comment { get; set; }

        public Guid? WarehouseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ContactId { get; set; }
    }

    public class OrderResponseDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = default!;
        public DateTime? ApprovedAt { get; set; }

        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }

        public Guid WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public Guid? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }

        public Guid? ContactId { get; set; }
        public string? ContactName { get; set; }

        public string? Comment { get; set; }
    }

    public class ApprovedOrderDto
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
}
