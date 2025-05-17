using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStock.DTO;
using MyStock.Entities;
using MyStock.Extensions;
using MyStock.Utils;

namespace MyStock.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
            => _context = context;

        private IQueryable<OrderDto> OrderProjection =>
            _context.Orders
                .AsNoTracking()
                .Include(o => o.Organization)
                .Include(o => o.Contact)
                .Include(o => o.Warehouse)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Number = o.Number,
                    ApprovedAt = o.ApprovedAt,
                    Type = o.Type.ToCodeDisplay(),
                    Status = o.Status.ToCodeDisplay(),
                    Organization = o.Organization != null ? o.Organization.ToRef() : null,
                    Contact = o.Contact != null ? o.Contact.ToRef() : null,
                    Warehouse = o.Warehouse != null ? o.Warehouse.ToRef() : null,
                    Comment = o.Comment
                });

        /// <summary>
        /// Все заказы
        /// </summary>
        public async Task<List<OrderDto>> GetAllAsync()
            => await OrderProjection.ToListAsync();

        /// <summary>
        /// Заказ по Id
        /// </summary>
        public async Task<OrderDto?> GetByIdAsync(Guid id)
            => await OrderProjection
                .FirstOrDefaultAsync(o => o.Id == id);

        /// <summary>
        /// Создать заказ
        /// </summary>
        public async Task<Guid> CreateAsync(CreateOrderDto dto)
        {
            EnumUtils.EnsureEnumDefined(dto.Type, nameof(dto.Type));
            EnumUtils.EnsureEnumDefined(dto.Status, nameof(dto.Status));

            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.ContactId, "Контакт");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                Number = dto.Number,
                Type = dto.Type,
                Status = dto.Status,
                Comment = dto.Comment,
                ApprovedAt = null,
                WarehouseId = dto.WarehouseId!.Value,
                OrganizationId = dto.OrganizationId,
                ContactId = dto.ContactId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order.Id;
        }

        /// <summary>Обновить заказ</summary>
        public async Task<bool> UpdateAsync(Guid id, CreateOrderDto dto)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return false;

            EnumUtils.EnsureEnumDefined(dto.Type, nameof(dto.Type));
            EnumUtils.EnsureEnumDefined(dto.Status, nameof(dto.Status));

            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.ContactId, "Контакт");

            o.Number = dto.Number;
            o.Type = dto.Type;
            o.Status = dto.Status;
            o.Comment = dto.Comment;
            o.WarehouseId = dto.WarehouseId!.Value;
            o.OrganizationId = dto.OrganizationId;
            o.ContactId = dto.ContactId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return false;

            _context.Orders.Remove(o);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Подтвердить заказ
        /// </summary>
        public async Task<ApprovedOrderDto> ApproveAsync(Guid id)
        {
            var o = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (o == null)
                throw new KeyNotFoundException("Заказ не найден");

            if (o.Status != OrderStatus.Draft)
                throw new InvalidOperationException("Можно подтверждать только черновики");

            await using var tx = await _context.Database.BeginTransactionAsync();

            foreach (var item in o.Items)
            {
                var inv = await _context.OrderItems
                    .FirstOrDefaultAsync(ii => ii.ProductId == item.ProductId);
                if (inv == null)
                    throw new InvalidOperationException($"Нет запаса для товара {item.ProductId}");

                if (o.Type == OrderType.Incoming)
                    inv.Quantity += item.Quantity;
                else
                {
                    if (inv.Quantity < item.Quantity)
                        throw new InvalidOperationException(
                            $"Недостаточно запаса {inv.Quantity} < {item.Quantity}");
                    inv.Quantity -= item.Quantity;
                }
            }

            o.Status = OrderStatus.Approved;
            o.ApprovedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new ApprovedOrderDto
            {
                Id = o.Id,
                Status = o.Status.ToCodeDisplay(),
                ApprovedAt = o.ApprovedAt!.Value
            };
        }
    }
}
