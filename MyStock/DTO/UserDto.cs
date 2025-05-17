using MyStock.DTO;
using MyStock.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyStock.DTO
{
    public class CreateUserDto
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = default!;

        public Guid? ContactId { get; set; }
    }
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        public CodeDisplayDto Role { get; set; } = default!;
        public ReferenceDto? Contact { get; set; }
    }
    public class LoginDto
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
    public class LoginResultDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public CodeDisplayDto Role { get; set; } = default!;
    }
}
