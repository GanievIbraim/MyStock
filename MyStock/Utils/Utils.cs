using Microsoft.EntityFrameworkCore;
using MyStock.Entities;

namespace MyStock.Utils
{
    public class ServiceUtils
    {
        /// <summary>
        /// Проверка существования связанной сущности по Id
        /// </summary>
        public static async Task EnsureExistsAsync<TEntity>(DbSet<TEntity> dbSet, Guid? id, string entityName) where TEntity : BaseEntity
        {
            if (!id.HasValue) return;
            var exists = await dbSet.AsNoTracking().AnyAsync(e => e.Id == id.Value);
            if (!exists)
                throw new KeyNotFoundException($"{entityName} с Id = {id} не найден");
        }
    }

    /// <summary>
    /// Проверяет, что enum-значение определено
    /// </summary>
    public class EnumUtils
    {
        public static void EnsureEnumDefined<TEnum>(TEnum value, string paramName)
            where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new ArgumentException($"Недопустимое значение {paramName}: {value}", paramName);
        }
    }
}
