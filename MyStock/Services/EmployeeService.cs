using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStock.Entities;
using MyStock.DTO;
using MyStock.Extensions;
using MyStock.Utils;

namespace MyStock.Services
{
    public class EmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<EmployeeDto> EmployeeProjection =>
            _context.Employees
                .AsNoTracking()
                .Include(e => e.Warehouse)
                .Include(e => e.Organization)
                .Include(e => e.Contact)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Patronymic = e.Patronymic,
                    Position = e.Position,
                    Email = e.Email,
                    Phone = e.Phone,
                    Status = e.Status.ToCodeDisplay(),
                    DateOfBirth = e.DateOfBirth,
                    Warehouse = e.Warehouse != null ? e.Warehouse.ToRef() : null,
                    Organization = e.Organization != null ? e.Organization.ToRef() : null,
                    Contact = e.Contact != null ? e.Contact.ToRef() : null
                });

        /// <summary>
        /// Получить всех сотрудников.
        /// Для пейджинга можно добавить параметры pageNumber/pageSize.
        /// </summary>
        public async Task<List<EmployeeDto>> GetAllAsync()
            => await EmployeeProjection.ToListAsync();

        /// <summary>
        /// Получить одного сотрудника по Id.
        /// </summary>
        public async Task<EmployeeDto?> GetByIdAsync(Guid id)
            => await EmployeeProjection.FirstOrDefaultAsync(e => e.Id == id);

        /// <summary>
        /// Создать нового сотрудника.
        /// </summary>
        public async Task<Guid> CreateAsync(CreateEmployeeDto dto)
        {
            EnumUtils.EnsureEnumDefined(dto.Status, nameof(dto.Status));

            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.ContactId, "Контакт");

            var emp = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Patronymic = dto.Patronymic,
                Position = dto.Position,
                Email = dto.Email,
                Phone = dto.Phone,
                Status = dto.Status,
                DateOfBirth = dto.DateOfBirth,
                WarehouseId = dto.WarehouseId,
                OrganizationId = dto.OrganizationId,
                ContactId = dto.ContactId
            };

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            return emp.Id;
        }

        /// <summary>
        /// Обновить данные сотрудника. Возвращает true, если сотрудник найден и обновлён.
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, CreateEmployeeDto dto)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return false;

            EnumUtils.EnsureEnumDefined(dto.Status, nameof(dto.Status));
            await ServiceUtils.EnsureExistsAsync(_context.Warehouses, dto.WarehouseId, "Склад");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.OrganizationId, "Организация");
            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.ContactId, "Контакт");

            emp.FirstName = dto.FirstName;
            emp.LastName = dto.LastName;
            emp.Patronymic = dto.Patronymic;
            emp.Position = dto.Position;
            emp.Email = dto.Email;
            emp.Phone = dto.Phone;
            emp.Status = dto.Status;
            emp.DateOfBirth = dto.DateOfBirth;
            emp.WarehouseId = dto.WarehouseId;
            emp.OrganizationId = dto.OrganizationId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить сотрудника по Id. Возвращает true, если была удалена запись.
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return false;

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
