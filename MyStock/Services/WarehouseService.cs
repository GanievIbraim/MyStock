using MyStock.DTO;
using MyStock.Entities;
using MyStock.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MyStock.Services
{
    public class WarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
        {
            return await _context.Warehouses
                .Include(w => w.Owner)
                .Select(w => new WarehouseDto
                {
                    Name = w.Name,
                    Address = w.Address,
                    Description = w.Description,
                    Owner = w.Owner.ToRef()


                })
                .ToListAsync();
        }

        public async Task<WarehouseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Warehouses
                .Include(w => w.Owner)
                .Where(w => w.Id == id)
                .Select(w => new WarehouseDto
                {
                    Name = w.Name,
                    Address = w.Address,
                    Description = w.Description,
                    Owner = w.Owner.ToRef()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Warehouse> CreateAsync(CreateWarehouseDto dto)
        {
            if (dto.OwnerId == null || !await _context.Users.AnyAsync(u => u.Id == dto.OwnerId))
                throw new KeyNotFoundException("Пользователь (владелец) не найден");

            var entity = new Warehouse
            {
                Name = dto.Name,
                Address = dto.Address,
                Description = dto.Description,
                OwnerId = dto.OwnerId.Value
            };

            _context.Warehouses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return false;

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Warehouse?> UpdateAsync(Guid id, CreateWarehouseDto dto)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null) return null;

            if (dto.OwnerId == null || !await _context.Users.AnyAsync(u => u.Id == dto.OwnerId))
                throw new KeyNotFoundException("Пользователь (владелец) не найден");

            warehouse.Name = dto.Name;
            warehouse.Address = dto.Address;
            warehouse.Description = dto.Description;
            warehouse.OwnerId = dto.OwnerId.Value;

            await _context.SaveChangesAsync();
            return warehouse;
        }
    }
}
