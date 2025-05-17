using MyStock.DTO;
using MyStock.Entities;

namespace MyStock.Extensions
{
    public static class ReferenceExtensions
    {
        public static ReferenceDto ToRef(this User user)
            => new() { Id = user.Id, DisplayValue = user.Login };

        public static ReferenceDto ToRef(this Organization org)
            => new() { Id = org.Id, DisplayValue = org.Name };

        public static ReferenceDto ToRef(this ProductCategory cat)
            => new() { Id = cat.Id, DisplayValue = cat.Name };

        public static ReferenceDto ToRef(this Contact contact)
            => new() { Id = contact.Id, DisplayValue = contact.FullName };

        public static ReferenceDto ToRef(this Warehouse warehouse)
            => new() { Id = warehouse.Id, DisplayValue = warehouse.Name };

        public static ReferenceDto ToRef(this WarehouseSection section)
            => new() { Id = section.Id, DisplayValue = section.Code };
    }
}
