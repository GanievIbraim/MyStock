using MyStock.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStock.Entities
{
    public class Order : BaseEntity
    {
        public string Number { get; set; } = default!;
        public DateTime? ApprovedAt { get; set; }

        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }

        public Guid CreatedById { get; set; }
        public Contact CreatedBy { get; set; } = default!;
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public Guid? ContactId { get; set; }
        public Contact? Contact { get; set; }
        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;
        public string? Comment { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
    public enum OrderType
    {
        [Display(Name = "Приход")]
        Incoming = 0,

        [Display(Name = "Расход")]
        Outgoing = 1,

        [Display(Name = "Списание")]
        WriteOff = 2
    }

    [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
    public enum OrderStatus
    {
        [Display(Name = "Черновик")]
        Draft = 0,

        [Display(Name = "Проведён")]
        Approved = 1,

        [Display(Name = "Отменён")]
        Cancelled = 2
    }
}
