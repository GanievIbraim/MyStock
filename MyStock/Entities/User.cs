﻿using MyStock.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyStock.Entities
{
    public class User : BaseEntity
    {
        public string PasswordHash { get; set; } = default!;
        public string Login { get; set; } = default!;
        public UserRole Role { get; set; } = default!;
        public bool IsActive { get; set; } = true;

        public Guid? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }

    [JsonConverter(typeof(JsonCamelCaseEnumConverter))]
    public enum UserRole
    {
        [Display(Name = "Администратор")]
        Administrator = 0,

        [Display(Name = "Кладовщик")]
        Storekeeper = 1,

        [Display(Name = "Менеджер")]
        Manager = 2,

        [Display(Name = "Пользователь")]
        ExternalUser = 3
    }

}
