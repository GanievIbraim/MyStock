using Microsoft.EntityFrameworkCore;

namespace MyStock.Entities
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseSection> WarehouseSections { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default timestamps for BaseEntity
            var baseEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType))
                .ToList();

            foreach (var type in baseEntityTypes)
            {
                modelBuilder.Entity(type.ClrType)
                    .Property("CreatedAt").HasDefaultValueSql("now()");
                modelBuilder.Entity(type.ClrType)
                    .Property("UpdatedAt").HasDefaultValueSql("now()");
            }

            // Contact - Organization (1:N)
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Organization)
                .WithMany(o => o.Contacts)
                .HasForeignKey(c => c.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization.PrimaryContact (1:1)
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.PrimaryContact)
                .WithMany()
                .HasForeignKey("PrimaryContactId")
                .OnDelete(DeleteBehavior.SetNull);

            // WarehouseSection - Warehouse (N:1)
            modelBuilder.Entity<WarehouseSection>()
                .HasOne(ws => ws.Warehouse)
                .WithMany(w => w.Sections)
                .HasForeignKey(ws => ws.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Order (N:1)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Product (N:1)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // InventoryItem - Inventory (N:1)
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.Inventory)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // InventoryItem - Product (N:1)
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.Product)
                .WithMany()
                .HasForeignKey(ii => ii.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // InventoryItem - WarehouseSection (N:1)
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.Section)
                .WithMany()
                .HasForeignKey(ii => ii.SectionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Employee - Contact (N:1)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Contact)
                .WithMany()
                .HasForeignKey(e => e.ContactId)
                .OnDelete(DeleteBehavior.SetNull);

            // 1) Кто создал заказ — это Contact
            modelBuilder.Entity<Order>()
                .HasOne(o => o.CreatedBy)
                .WithMany()                          // или .WithMany(c => c.CreatedOrders) если хотите собрать коллекцию
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // 2) Бизнес-контакт по заказу
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Contact)
                .WithMany()                          // или .WithMany(c => c.OrdersAsContact)
                .HasForeignKey(o => o.ContactId)
                .OnDelete(DeleteBehavior.SetNull);


            // Precision for price fields
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

            // Indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique();
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Barcode);

            // Enum stored as string
            modelBuilder.Entity<Order>()
                .Property(o => o.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Inventory>()
                .Property(i => i.Type)
                .HasConversion<string>();
            modelBuilder.Entity<Inventory>()
                .Property(i => i.Status)
                .HasConversion<string>();

            // String length constraints
            modelBuilder.Entity<Contact>()
                .Property(c => c.FullName)
                .HasMaxLength(200)
                .IsRequired();
            modelBuilder.Entity<Organization>()
                .Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in modified)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in modified)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}
