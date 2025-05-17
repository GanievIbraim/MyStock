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
    public class WarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
            => _context = context;

        private IQueryable<WarehouseDto> WarehouseProjection =>
            _context.Warehouses
                .AsNoTracking()
                .Include(w => w.Owner)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Address = w.Address,
                    Description = w.Description,
                    Owner = w.Owner != null ? w.Owner.ToRef() : null
                });

        /// <summary>
        /// Получить все склады
        /// </summary>
        public async Task<List<WarehouseDto>> GetAllAsync()
            => await WarehouseProjection.ToListAsync();

        /// <summary>
        /// Получить склад по Id
        /// </summary>
        public async Task<WarehouseDto?> GetByIdAsync(Guid id)
            => await WarehouseProjection
                .FirstOrDefaultAsync(w => w.Id == id);

        /// <summary>
        /// Создать новый склад (возвращает Id)
        /// </summary>
        public async Task<Guid> CreateAsync(CreateWarehouseDto dto)
        {
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.OwnerId, "Пользователь (владелец)");

            var entity = new Warehouse
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Description = dto.Description,
                OwnerId = dto.OwnerId!.Value
            };

            _context.Warehouses.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Обновить склад. Возвращает true, если найден и обновлён.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, CreateWarehouseDto dto)
        {
            var w = await _context.Warehouses.FindAsync(id);
            if (w == null) return false;

            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.OwnerId, "Пользователь (владелец)");

            w.Name = dto.Name;
            w.Address = dto.Address;
            w.Description = dto.Description;
            w.OwnerId = dto.OwnerId!.Value;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить склад. Возвращает true, если удалён.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var w = await _context.Warehouses.FindAsync(id);
            if (w == null) return false;

            _context.Warehouses.Remove(w);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
