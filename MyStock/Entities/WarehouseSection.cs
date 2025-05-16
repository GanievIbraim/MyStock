using MyStock.Entities;

public class WarehouseSection : BaseEntity
{
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;
}
