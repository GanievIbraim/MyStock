namespace MyStock.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public Guid? SectionId { get; set; }
        public WarehouseSection? Section { get; set; }
    }
}
