using MyStock.DTO;
using MyStock.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

    [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
    public enum OrganizationType
    {
        [Display(Name = "Покупатель")]
        Customer = 0,

        [Display(Name = "Поставщик")]
        Supplier = 1,

        [Display(Name = "Партнер")]
        Partner = 2,

        [Display(Name = "Подрядчик")]
        Contractor = 3
    }
}
