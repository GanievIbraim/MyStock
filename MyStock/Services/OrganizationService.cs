using MyStock.Entities;
using Microsoft.EntityFrameworkCore;

namespace MyStock.Services
{
    public class OrganizationService : IService
    {
        private readonly AppDbContext _context;

        public OrganizationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _context.Organizations
                .Include(o => o.PrimaryContact)
                .ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(Guid id)
        {
            return await _context.Organizations
                .Include(o => o.PrimaryContact)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Organization>> FilterByTypeAsync(OrganizationType type)
        {
            return await _context.Organizations
                .Where(o => o.Type == type)
                .ToListAsync();
        }

        public async Task<Organization> CreateAsync(Organization entity)
        {
            _context.Organizations.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Organization?> UpdateAsync(Guid id, Organization updated)
        {
            var existing = await _context.Organizations.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(updated);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var org = await _context.Organizations.FindAsync(id);
            if (org == null) return false;

            _context.Organizations.Remove(org);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
