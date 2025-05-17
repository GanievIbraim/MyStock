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
    public class OrganizationService
    {
        private readonly AppDbContext _context;

        public OrganizationService(AppDbContext context)
            => _context = context;

        // ─── Проекция Organization → OrganizationDto ──────────────────────
        private IQueryable<OrganizationDto> OrganizationProjection =>
            _context.Organizations
                .AsNoTracking()
                .Include(o => o.PrimaryContact)
                .Select(o => new OrganizationDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    LegalForm = o.LegalForm,
                    INN = o.INN,
                    KPP = o.KPP,
                    OGRN = o.OGRN,
                    Address = o.Address,
                    Phone = o.Phone,
                    Email = o.Email,
                    Type = o.Type.ToCodeDisplay(),
                    PrimaryContact = o.PrimaryContact != null ? o.PrimaryContact.ToRef() : null,
                });

        /// <summary>
        /// Все организации
        /// </summary>
        public async Task<List<OrganizationDto>> GetAllAsync()
            => await OrganizationProjection.ToListAsync();

        /// <summary>
        /// Организация по Id
        /// </summary>
        public async Task<OrganizationDto?> GetByIdAsync(Guid id)
            => await OrganizationProjection
                .FirstOrDefaultAsync(o => o.Id == id);

        /// <summary>
        /// Фильтр по типу организации
        /// </summary>
        public async Task<List<OrganizationDto>> FilterByTypeAsync(OrganizationType type)
            => await _context.Organizations
                .Where(o => o.Type == type)
                .Select(o => new OrganizationDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Type = o.Type.ToCodeDisplay(),
                    PrimaryContact = o.PrimaryContact != null ? o.PrimaryContact.ToRef() : null,
                })
                .ToListAsync();

        /// <summary>
        /// Создать новую организацию
        /// </summary>
        public async Task<Guid> CreateAsync(CreateOrganizationDto dto)
        {
            EnumUtils.EnsureEnumDefined(dto.Type, nameof(dto.Type));

            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.PrimaryContactId, "Контакт");

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                LegalForm = dto.LegalForm,
                INN = dto.INN,
                KPP = dto.KPP,
                OGRN = dto.OGRN,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email,
                Type = dto.Type,
                PrimaryContactId = dto.PrimaryContactId
            };

            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();
            return org.Id;
        }

        /// <summary>Обновить организацию. Возвращает true, если найдена и обновлена.</summary>
        public async Task<bool> UpdateAsync(Guid id, CreateOrganizationDto dto)
        {
            var org = await _context.Organizations.FindAsync(id);
            if (org == null) return false;

            EnumUtils.EnsureEnumDefined(dto.Type, nameof(dto.Type));
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.PrimaryContactId, "Контакт");

            // Явно присваиваем, чтобы не трогать другие поля
            org.Name = dto.Name;
            org.LegalForm = dto.LegalForm;
            org.INN = dto.INN;
            org.KPP = dto.KPP;
            org.OGRN = dto.OGRN;
            org.Address = dto.Address;
            org.Phone = dto.Phone;
            org.Email = dto.Email;
            org.Type = dto.Type;
            org.PrimaryContactId = dto.PrimaryContactId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить организацию по Id. Возвращает true, если удалена.
        /// </summary>
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
