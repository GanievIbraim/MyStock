using System.ComponentModel.DataAnnotations;

namespace MyStock.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public ProductUnit Unit { get; set; }

        public Guid CategoryId { get; set; }
        public ProductCategory Category { get; set; } = default!;

        public Guid? SectionId { get; set; }
        public WarehouseSection? Section { get; set; }

        public Guid? SupplierId { get; set; }
        public Organization? Supplier { get; set; }

    }
    
    public enum ProductUnit
    {
        [Display(Name = "Штука")]
        Piece = 0,

        [Display(Name = "Килограмм")]
        Kilogram = 1,

        [Display(Name = "Литр")]
        Liter = 2,

        [Display(Name = "Пачка")]
        Pack = 3,

        [Display(Name = "Метры")]
        Meter = 4
    }

}