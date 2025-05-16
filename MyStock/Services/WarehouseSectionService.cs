using MyStock.DTO;
using MyStock.Entities;
using MyStock.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MyStock.Services
{
    public class WarehouseSectionService : IService
    {
        private readonly AppDbContext _context;

        public WarehouseSectionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WarehouseSectionDto>> GetAllAsync()
        {
            return await _context.WarehouseSections
                .Include(s => s.Warehouse)
                .Select(s => new WarehouseSectionDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    Warehouse = s.Warehouse.ToRef()
                })
                .ToListAsync();
        }

        public async Task<WarehouseSectionDto?> GetByIdAsync(Guid id)
        {
            return await _context.WarehouseSections
                .Include(s => s.Warehouse)
                .Where(s => s.Id == id)
                .Select(s => new WarehouseSectionDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Description = s.Description,
                    Warehouse = s.Warehouse.ToRef()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<WarehouseSection> CreateAsync(CreateWarehouseSectionDto dto)
        {
            var exists = await _context.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId);
            if (!exists)
                throw new KeyNotFoundException("Склад не найден");

            var section = new WarehouseSection
            {
                Code = dto.Code,
                Description = dto.Description,
                WarehouseId = dto.WarehouseId
            };

            _context.WarehouseSections.Add(section);
            await _context.SaveChangesAsync();
            return section;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var section = await _context.WarehouseSections.FindAsync(id);
            if (section == null) return false;

            _context.WarehouseSections.Remove(section);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WarehouseSection?> UpdateAsync(Guid id, CreateWarehouseSectionDto dto)
        {
            var section = await _context.WarehouseSections.FindAsync(id);
            if (section == null) return null;

            var exists = await _context.Warehouses.AnyAsync(w => w.Id == dto.WarehouseId);
            if (!exists)
                throw new KeyNotFoundException("Склад не найден");

            section.Code = dto.Code;
            section.Description = dto.Description;
            section.WarehouseId = dto.WarehouseId;

            await _context.SaveChangesAsync();
            return section;
        }
    }
}
