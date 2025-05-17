using Microsoft.EntityFrameworkCore;
using MyStock.Dto;
using MyStock.Entities;
using MyStock.Extensions;
using MyStock.Utils;

namespace MyStock.Services
{
    public class ContactService
    {
        private readonly AppDbContext _context;
        public ContactService(AppDbContext context) => _context = context;

        private IQueryable<ContactDto> ContactProjection =>
            _context.Contacts
                .AsNoTracking()
                .Include(c => c.Organization)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Position = c.Position,
                    Phone = c.Phone,
                    Email = c.Email,
                    Organization = c.Organization != null ? c.Organization.ToRef() : null,
                });

        /// <summary>
        /// Получить всех контактов
        /// </summary>
        public async Task<List<ContactDto>> GetAllAsync()
            => await ContactProjection.ToListAsync();

        /// <summary>
        /// Получить контакт по Id
        /// </summary>
        public async Task<ContactDto?> GetByIdAsync(Guid id)
            => await ContactProjection.FirstOrDefaultAsync(c => c.Id == id);

        /// <summary>
        /// Создать новый контакт (возвращает его Id)
        /// </summary>
        public async Task<Guid> CreateAsync(CreateContactDto dto)
        {
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");

            var contact = new Contact
            {
                FullName = dto.FullName,
                Position = dto.Position,
                Phone = dto.Phone,
                Email = dto.Email,
                OrganizationId = dto.OrganizationId
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id;
        }

        /// <summary>
        /// Обновить контакт. Возвращает true, если найден и обновлён.
        /// s</summary>
        public async Task<bool> UpdateAsync(Guid id, CreateContactDto dto)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return false;

            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");

            contact.FullName = dto.FullName;
            contact.Position = dto.Position;
            contact.Phone = dto.Phone;
            contact.Email = dto.Email;
            contact.OrganizationId = dto.OrganizationId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить контакт. Возвращает true, если удалён.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return false;

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
