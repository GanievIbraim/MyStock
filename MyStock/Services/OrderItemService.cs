using Microsoft.EntityFrameworkCore;
using MyStock.Entities;
using MyStock.DTO;

namespace MyStock.Services
{
    public class OrderItemService : IService
    {
        private readonly AppDbContext _context;

        public OrderItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> CreateAsync(CreateOrderItemDto dto)
        {
            // Проверка существования заказа
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null)
                throw new ArgumentException("Заказ не найден");

            // Проверка существования товара
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new ArgumentException("Товар не найден");

            // Проверка секции (опционально)
            WarehouseSection section = null;
            if (dto.SectionId.HasValue)
            {
                section = await _context.WarehouseSections.FindAsync(dto.SectionId.Value);
                if (section == null)
                    throw new ArgumentException("Секция не найдена");
            }

            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = (int)dto.Quantity,
                Price = dto.Price,
                SectionId = dto.SectionId
            };

            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<OrderItem>> CreateManyAsync(IEnumerable<CreateOrderItemDto> dtos)
        {
            var items = new List<OrderItem>();
            foreach (var dto in dtos)
            {
                var item = await CreateAsync(dto);
                items.Add(item);
            }
            return items;
        }

        public async Task<OrderItem?> UpdateAsync(Guid id, CreateOrderItemDto dto)
        {
            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null)
                return null;

            // Валидация аналогично CreateAsync
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null)
                throw new ArgumentException("Заказ не найден");

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                throw new ArgumentException("Товар не найден");

            if (dto.SectionId.HasValue)
            {
                var section = await _context.WarehouseSections.FindAsync(dto.SectionId.Value);
                if (section == null)
                    throw new ArgumentException("Секция не найдена");
            }

            existing.OrderId = dto.OrderId;
            existing.ProductId = dto.ProductId;
            existing.Quantity = (int)dto.Quantity;
            existing.Price = dto.Price;
            existing.SectionId = dto.SectionId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null)
                return false;

            _context.OrderItems.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<OrderItemDto>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems
                .Where(i => i.OrderId == orderId)
                .Include(i => i.Product)
                .Include(i => i.Order)
                .Include(i => i.Section)
                .Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    OrderId = i.OrderId,
                    OrderName = i.Order.Number,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    SectionId = i.SectionId,
                    SectionName = i.Section != null ? i.Section.Code : string.Empty
                })
                .ToListAsync();
        }
    }
}