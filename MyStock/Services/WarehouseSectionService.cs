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
    public class WarehouseSectionService
    {
        private readonly AppDbContext _context;

        public WarehouseSectionService(AppDbContext context)
            => _context = context;

        private IQueryable<WarehouseSectionDto> SectionProjection =>
            _context.WarehouseSections
                .AsNoTracking()
                .Include(s => s.Warehouse)
                .Select(s => new WarehouseSectionDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    Warehouse = s.Warehouse.ToRef()
                });

        /// <summary>
        /// Получить все секции
        /// </summary>
        public async Task<List<WarehouseSectionDto>> GetAllAsync()
            => await SectionProjection.ToListAsync();

        /// <summary>
        /// Получить секцию по Id
        /// </summary>
        public async Task<WarehouseSectionDto?> GetByIdAsync(Guid id)
            => await SectionProjection
                .FirstOrDefaultAsync(s => s.Id == id);

        /// <summary>
        /// Создать новую секцию (возвращает её Id)
        /// </summary>
        public async Task<Guid> CreateAsync(CreateWarehouseSectionDto dto)
        {
            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");

            var section = new WarehouseSection
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Description = dto.Description,
                WarehouseId = dto.WarehouseId
            };

            _context.WarehouseSections.Add(section);
            await _context.SaveChangesAsync();
            return section.Id;
        }

        /// <summary>
        /// Обновить секцию. Возвращает true, если найдена и обновлена.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, CreateWarehouseSectionDto dto)
        {
            var section = await _context.WarehouseSections.FindAsync(id);
            if (section == null)
                return false;

            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");

            section.Code = dto.Code;
            section.Description = dto.Description;
            section.WarehouseId = dto.WarehouseId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить секцию. Возвращает true, если удалена.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var section = await _context.WarehouseSections.FindAsync(id);
            if (section == null)
                return false;

            _context.WarehouseSections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
