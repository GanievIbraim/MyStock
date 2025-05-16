using MyStock.DTO;
using MyStock.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MyStock.Services
{
    public class OrderService : IService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Contact)
                .Include(o => o.Warehouse)
                .Include(o => o.Organization)
                .Select(OrderMappings.ToResponseDto())
                .ToListAsync();
        }

        public async Task<OrderResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Contact)
                .Include(o => o.Warehouse)
                .Include(o => o.Organization)
                .Where(o => o.Id == id)
                .Select(OrderMappings.ToResponseDto())
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderResponseDto>> FilterByTypeAsync(OrganizationType type)
        {
            return await _context.Orders
                .Include(o => o.Contact)
                .Include(o => o.Warehouse)
                .Include(o => o.Organization)
                .Where(o => o.Organization != null && o.Organization.Type == type)
                .Select(OrderMappings.ToResponseDto())
                .ToListAsync();
        }

        public async Task<Order> CreateAsync(CreateOrderDto dto)
        {
            Guid warehouseId = await CheckWarkhouse(dto.WarehouseId.Value);
            Guid organizationId = await CheckOrganization(dto.OrganizationId.Value);
            Guid contactId = await CheckContact(dto.ContactId.Value);

            if (contactId == Guid.Empty && organizationId == Guid.Empty)
            {
                throw new ArgumentException("Необходимо указать хотя бы организацию или контакт.");
            }

            var order = new Order
            {
                Number = dto.Number,
                Type = dto.Type,
                Status = OrderStatus.Draft,
                Comment = dto.Comment,
                ApprovedAt = null,
                WarehouseId = warehouseId,
                OrganizationId = organizationId,
                ContactId = contactId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> UpdateAsync(Guid id, CreateOrderDto dto)
        {
            var existing = await _context.Orders.FindAsync(id);
            if (existing == null) return null;

            Guid warehouseId = await CheckWarkhouse(dto.WarehouseId.Value);
            Guid organizationId = await CheckOrganization(dto.OrganizationId.Value);
            Guid contactId = await CheckContact(dto.ContactId.Value);

            if (contactId == Guid.Empty && organizationId == Guid.Empty)
                throw new ArgumentException("Необходимо указать хотя бы организацию или контакт.");

            existing.Number = dto.Number;
            existing.Type = dto.Type;
            existing.Comment = dto.Comment;
            existing.WarehouseId = warehouseId;
            existing.OrganizationId = organizationId;
            existing.ContactId = contactId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ApprovedOrderDto> ApproveAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException($"Order {orderId} not found.");

            if (order.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Only Draft orders can be approved.");

            await using var tx = await _context.Database.BeginTransactionAsync();

            foreach (var item in order.Items)
            {
                var orderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(ii => ii.ProductId == item.ProductId);

                if (orderItem == null)
                    throw new InvalidOperationException($"No inventory record for product {item.ProductId}");

                switch (order.Type)
                {
                    case OrderType.Incoming:
                        orderItem.Quantity += item.Quantity;
                        break;

                    case OrderType.Outgoing:
                    case OrderType.WriteOff:
                        if (orderItem.Quantity < item.Quantity)
                            throw new InvalidOperationException(
                                $"Insufficient stock for product {item.ProductId}: available {orderItem.Quantity}, required {item.Quantity}");

                        orderItem.Quantity -= item.Quantity;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(order.Type), order.Type, "Unknown order type");
                }
            }

            order.Status = OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new ApprovedOrderDto
            {
                Id = order.Id,
                Status = order.Status,
                ApprovedAt = order.ApprovedAt.Value
            };
        }

        public async Task<Guid> CheckWarkhouse(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            return warehouse?.Id ?? Guid.Empty;
        }

        public async Task<Guid> CheckOrganization(Guid id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            return organization?.Id ?? Guid.Empty;
        }
        public async Task<Guid> CheckContact(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            return contact?.Id ?? Guid.Empty;
        }
    }

    public static class OrderMappings
    {
        public static Expression<Func<Order, OrderResponseDto>> ToResponseDto()
        {
            return o => new OrderResponseDto
            {
                Id = o.Id,
                Type = o.Type,
                Status = o.Status,
                Number = o.Number,
                Comment = o.Comment,
                ContactId = o.ContactId,
                ApprovedAt = o.ApprovedAt,
                ContactName = o.Contact != null ? o.Contact.FullName : null,
                WarehouseId = o.WarehouseId,
                WarehouseName = o.Warehouse != null ? o.Warehouse.Name : string.Empty,
                OrganizationId = o.OrganizationId,
                OrganizationName = o.Organization != null ? o.Organization.Name : string.Empty
            };
        }
    }
}
