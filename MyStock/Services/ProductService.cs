using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStock.DTO;
using MyStock.Utils;
using MyStock.Entities;
using MyStock.Extensions;

namespace MyStock.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
            => _context = context;

        private IQueryable<ProductDto> ProductProjection =>
            _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Section)
                .Include(p => p.Supplier)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Barcode = p.Barcode,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Unit = p.Unit.ToCodeDisplay(),
                    Category = p.Category != null ? p.Category.ToRef() : null,
                    Section = p.Section != null ? p.Section.ToRef() : null,
                    Supplier = p.Supplier != null ? p.Supplier.ToRef() : null,
                });

        /// <summary>
        /// Получить все товары
        /// </summary>
        public async Task<List<ProductDto>> GetAllAsync()
            => await ProductProjection.ToListAsync();

        /// <summary>
        /// Получить товар по Id
        /// </summary>
        public async Task<ProductDto?> GetByIdAsync(Guid id)
            => await ProductProjection
                .FirstOrDefaultAsync(p => p.Id == id);

        /// <summary>
        /// Получить товар по штрихкоду
        /// </summary>
        public async Task<ProductDto?> GetByBarcodeAsync(string barcode)
            => await ProductProjection
                .FirstOrDefaultAsync(p => p.Barcode == barcode);

        /// <summary>
        /// Создать новый товар
        /// </summary>
        public async Task<Guid> CreateAsync(CreateProductDto dto)
        {
            EnumUtils.EnsureEnumDefined(dto.Unit, nameof(dto.Unit));

            await ServiceUtils.EnsureExistsAsync(_context.ProductCategories, dto.CategoryId, "Категория");
            await ServiceUtils.EnsureExistsAsync(_context.WarehouseSections, dto.SectionId, "Секция склада");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.SupplierId, "Поставщик");

            var p = new Product
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Barcode = dto.Barcode,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Unit = dto.Unit,
                CategoryId = dto.CategoryId,
                SectionId = dto.SectionId,
                SupplierId = dto.SupplierId
            };

            _context.Products.Add(p);
            await _context.SaveChangesAsync();
            return p.Id;
        }

        /// <summary>
        /// Обновить существующий товар
        /// </summary>
        public async Task<bool> UpdateAsync(Guid id, CreateProductDto dto)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return false;

            EnumUtils.EnsureEnumDefined(dto.Unit, nameof(dto.Unit));
            await ServiceUtils.EnsureExistsAsync(_context.ProductCategories, dto.CategoryId, "Категория");
            await ServiceUtils.EnsureExistsAsync(_context.WarehouseSections, dto.SectionId, "Секция склада");
            await ServiceUtils.EnsureExistsAsync(_context.Organizations, dto.SupplierId, "Поставщик");

            p.Name = dto.Name;
            p.Code = dto.Code;
            p.Barcode = dto.Barcode;
            p.Description = dto.Description;
            p.Quantity = dto.Quantity;
            p.Price = dto.Price;
            p.Unit = dto.Unit;
            p.CategoryId = dto.CategoryId;
            p.SectionId = dto.SectionId;
            p.SupplierId = dto.SupplierId;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удалить товар по Id
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return false;

            _context.Products.Remove(p);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Фильтрация по имени, категории, секции
        /// </summary>
        public async Task<List<ProductDto>> FilterAsync(
            string? name,
            Guid? categoryId,
            Guid? sectionId)
        {
            var q = ProductProjection;

            if (!string.IsNullOrWhiteSpace(name))
                q = q.Where(p => p.Name.Contains(name));

            if (categoryId.HasValue)
                q = q.Where(p => p.Category != null && p.Category.Id == categoryId.Value);

            if (sectionId.HasValue)
                q = q.Where(p => p.Section != null && p.Section.Id == sectionId.Value);

            return await q.ToListAsync();
        }
    }
}
