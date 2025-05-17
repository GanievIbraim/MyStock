using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStock.DTO;
using MyStock.Entities;
using MyStock.Utils;

namespace MyStock.Services
{
    public class OrderItemService
    {
        private readonly AppDbContext _context;

        public OrderItemService(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<OrderItemDto> ItemProjection =>
            _context.OrderItems
                .AsNoTracking()
                .Include(i => i.Product)
                .Include(i => i.Order)
                .Include(i => i.Section)
                .Select(i => new OrderItemDto
                {
                    OrderId = i.OrderId,
                    OrderName = i.Order.Number,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    SectionId = i.SectionId,
                    SectionName = i.Section != null ? i.Section.Code : string.Empty
                });
       
        /// <summary>
        /// Получить все позиции по заданному заказу.
        /// </summary>
        public async Task<List<OrderItemDto>> GetByOrderIdAsync(Guid orderId)
        {
            await ServiceUtils.EnsureExistsAsync(_context.Orders, orderId, "Заказ");

            return await ItemProjection
                .Where(i => i.OrderId == orderId)
                .ToListAsync();
        }

        /// <summary>
        /// Получить позицию по Id.
        /// </summary>
        public async Task<OrderItemDto?> GetByIdAsync(Guid id)
            => await ItemProjection.FirstOrDefaultAsync(e => e.Id == id);

        /// <summary>
        /// Добавить одну позицию к заказу. Возвращает её Id.
        /// </summary>
        public async Task<Guid> CreateAsync(CreateOrderItemDto dto)
        {

            await ServiceUtils.EnsureExistsAsync(_context.Orders, dto.OrderId, "Заказ");
            await ServiceUtils.EnsureExistsAsync(_context.Products, dto.ProductId, "Товар");

            await ServiceUtils.EnsureExistsAsync(_context.WarehouseSections, dto.SectionId, "Секция склада");

            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = dto.OrderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = dto.Price,
                SectionId = dto.SectionId
            };

            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
            return item.Id;
        }

        /// <summary>
        /// Добавить сразу несколько позиций.
        /// </summary>
        public async Task<List<Guid>> CreateManyAsync(IEnumerable<CreateOrderItemDto> dtos)
        {
            var result = new List<Guid>();
            foreach (var dto in dtos)
            {
                var id = await CreateAsync(dto);
                result.Add(id);
            }
            return result;
        }

        /// <summary>
        /// Обновить существующую позицию. Возвращает true, если нашли и обновили.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, CreateOrderItemDto dto)
        {
            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null)
                return false;

            await ServiceUtils.EnsureExistsAsync(_context.Orders, dto.OrderId, "Заказ");
            await ServiceUtils.EnsureExistsAsync(_context.Products, dto.ProductId, "Товар");
            await ServiceUtils.EnsureExistsAsync(_context.WarehouseSections, dto.SectionId, "Секция склада");

            existing.OrderId = dto.OrderId;
            existing.ProductId = dto.ProductId;
            existing.Quantity = dto.Quantity;
            existing.Price = dto.Price;
            existing.SectionId = dto.SectionId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить позицию по Id. Возвращает true, если удалили.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.OrderItems.FindAsync(id);
            if (existing == null)
                return false;

            _context.OrderItems.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}