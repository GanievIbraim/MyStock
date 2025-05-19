using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStock.DTO;
using MyStock.Entities;
using MyStock.Extensions;
using MyStock.Utils;

namespace MyStock.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
            => _context = context;

        private IQueryable<UserDto> UserProjection =>
            _context.Users
                .AsNoTracking()
                .Include(u => u.Contact)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    Role = u.Role.ToCodeDisplay(),
                    IsActive = u.IsActive,
                    Contact = u.Contact != null ? u.Contact.ToRef() : null
                });

        /// <summary>
        /// Все пользователи
        /// </summary>
        public async Task<List<UserDto>> GetAllAsync()
            => await UserProjection.ToListAsync();

        /// <summary>
        /// Пользователь по Id
        /// </summary>
        public async Task<UserDto?> GetByIdAsync(Guid id)
            => await UserProjection
                .FirstOrDefaultAsync(u => u.Id == id);

        /// <summary>
        /// Создать пользователя (возвращает новый Id)
        /// </summary>
        public async Task<Guid> CreateAsync(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
                throw new InvalidOperationException("Пользователь с таким логином уже существует");

            EnumUtils.EnsureEnumDefined(dto.Role, nameof(dto.Role));

            await ServiceUtils.EnsureExistsAsync(_context.Contacts, dto.ContactId, "Контакт");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = dto.Login,
                PasswordHash = HashPassword(dto.Password),
                Role = dto.Role,
                IsActive = true,
                ContactId = dto.ContactId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        /// <summary>
        /// Включить/выключить активность (true – active)
        /// </summary>
        public async Task<bool> UpdateStatusAsync(Guid id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Попытка логина: возвращает Id, Login и роль в CodeDisplayDto
        /// </summary>
        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var hash = HashPassword(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Login == dto.Login &&
                    u.PasswordHash == hash &&
                    u.IsActive);

            if (user == null)
                throw new UnauthorizedAccessException("Неверный логин или пароль");

            return new LoginResultDto
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.Role.ToCodeDisplay()
            };
        }

        /// <summary>
        /// Удалить пользователя по Id
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Хэшируем пароль через SHA256 → Base64
        /// <summary>
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
