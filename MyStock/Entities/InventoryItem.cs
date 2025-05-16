namespace MyStock.Entities
{
    public class InventoryItem : BaseEntity
    {
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int ExpectedQuantity { get; set; } // по базе
        public int ActualQuantity { get; set; }   // по факту

        public Guid? SectionId { get; set; }
        public WarehouseSection? Section { get; set; }
    }

}
