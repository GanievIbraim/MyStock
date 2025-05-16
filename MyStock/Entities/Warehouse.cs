using MyStock.Entities;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public Contact Owner { get; set; } = default!;
    public ICollection<WarehouseSection> Sections { get; set; } = new List<WarehouseSection>();
}
