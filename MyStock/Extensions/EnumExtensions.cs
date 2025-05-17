using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using MyStock.DTO;

namespace MyStock.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            var attr = member?.GetCustomAttribute<DisplayAttribute>(false);
            return attr?.GetName() ?? value.ToString();
        }

        public static CodeDisplayDto ToCodeDisplay(this Enum value)
        {
            return new CodeDisplayDto
            {
                Code = value.ToString().ToCamelCase(),
                DisplayValue = value.GetDisplayName()
            };
        }

        // Утилита для camelCase
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}
