using System.ComponentModel.DataAnnotations;

namespace MyStock.Entities
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? LegalForm { get; set; }
        public string? INN { get; set; }
        public string? KPP { get; set; }
        public string? OGRN { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public OrganizationType Type { get; set; }
        public Guid? PrimaryContactId { get; set; }
        public Contact? PrimaryContact { get; set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
    public enum OrganizationType
    {
        [Display(Name = "Покупатель")]
        Customer,

        [Display(Name = "Поставщик")]
        Supplier,

        [Display(Name = "Партнер")]
        Partner,

        [Display(Name = "Подрядчик")]
        Contractor
    }
}
