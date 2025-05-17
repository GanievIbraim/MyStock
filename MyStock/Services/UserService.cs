using MyStock.DTO;
using MyStock.Entities;
using MyStock.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MyStock.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Contact)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    Contact = u.Contact != null ? u.Contact.ToRef() : null
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Contact)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                Contact = user.Contact?.ToRef()
            };
        }

        public async Task<User> CreateAsync(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
                throw new InvalidOperationException("Пользователь с таким логином уже существует");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = dto.Login,
                PasswordHash = HashPassword(dto.Password),
                Role = Enum.Parse<UserRole>(dto.Role, true),
                IsActive = true,
                ContactId = dto.ContactId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateStatus(Guid id, bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.IsActive = isActive;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<LoginResultDto> LoginAsync(LoginDto dto)
        {
            var passwordHash = HashPassword(dto.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Login == dto.Login &&
                    u.PasswordHash == passwordHash &&
                    u.IsActive);

            if (user == null)
                throw new UnauthorizedAccessException("Неверный логин или пароль");

            return new LoginResultDto
            {
                Id = user.Id,
                Login = user.Login,
                Role = user.Role.ToString()
            };
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
