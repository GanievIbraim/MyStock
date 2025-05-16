namespace MyStock.Entities
{
    public class Contact : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public Guid? OrganizationId { get; set; }
        public Organization? Organization { get; set; }
    }

}
