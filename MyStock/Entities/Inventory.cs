using System.ComponentModel.DataAnnotations;

namespace MyStock.Entities
{
    public class Inventory : BaseEntity
    {
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public Guid WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = default!;

        public Guid CreatedById { get; set; }
        public Contact CreatedBy { get; set; } = default!;

        public InventoryType Type { get; set; }
        public InventoryStatus Status { get; set; }
        public string? Comment { get; set; }
        public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
    }

    public enum InventoryStatus
    {
        [Display(Name = "Открыта")]
        Open,

        [Display(Name = "Завершена")]
        Completed,

        [Display(Name = "Подтверждена")]
        Approved
    }

    public enum InventoryType
    {
        [Display(Name = "Полная")]
        Full = 0,

        [Display(Name = "Выборочная")]
        Partial = 1,

        [Display(Name = "Плановая")]
        Scheduled = 2,

        [Display(Name = "Передача смены")]
        ShiftTransfer = 3,
    }
}
