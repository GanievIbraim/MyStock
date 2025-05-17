using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateOrderDto
    {
        [Required]
        public string Number { get; set; } = default!;

        [Required]
        public OrderType Type { get; set; } = default!;

        [Required]
        public OrderStatus Status { get; set; } = default!;
        public string? Comment { get; set; }

        public Guid? WarehouseId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? ContactId { get; set; }
    }
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = default!;
        public DateTime? ApprovedAt { get; set; }

        public CodeDisplayDto Type { get; set; } = default!;
        public CodeDisplayDto Status { get; set; } = default!;

        public ReferenceDto? Organization { get; set; }
        public ReferenceDto? Contact { get; set; }
        public ReferenceDto? Warehouse { get; set; }

        public string? Comment { get; set; }
    }
    public class ApprovedOrderDto
    {
        public Guid Id { get; set; }
        public CodeDisplayDto Status { get; set; } = default!;
        public DateTime ApprovedAt { get; set; }
    }
    public class OrderFilterParametersDto
    {
        public OrderStatus? Status { get; set; }
        public Guid? WarehouseId { get; set; }
        public DateTime? ApprovedFrom { get; set; }
        public DateTime? ApprovedTo { get; set; }
    }
}
