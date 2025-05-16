using System.ComponentModel.DataAnnotations;

namespace MyStock.Entities
{
    public class Order : BaseEntity
    {
        public string Number { get; set; } = default!;
        public DateTime? ApprovedAt { get; set; }

        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = default!;

        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public Guid? ContactId { get; set; }
        public Contact? Contact { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;
        public string? Comment { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
    public enum OrderType
    {
        [Display(Name = "Приход")]
        Incoming,

        [Display(Name = "Расход")]
        Outgoing,

        [Display(Name = "Списание")]
        WriteOff,
    }
    public enum OrderStatus
    {
        [Display(Name = "Черновик")]
        Draft,

        [Display(Name = "Проведён")]
        Approved,

        [Display(Name = "Отменён")]
        Cancelled
    }
}
