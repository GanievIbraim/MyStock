using MyStock.Entities;
using Microsoft.EntityFrameworkCore;
using MyStock.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.Include(p => p.Category).ToListAsync();

    public async Task<Product?> GetByIdAsync(Guid id) =>
        await _context.Products.FindAsync(id);

    public async Task<Product?> GetByBarcodeAsync(string barcode) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);

    public async Task<Product> CreateAsync(Product product)
    {
        var categoryExists = await _context.ProductCategories
            .AnyAsync(c => c.Id == product.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException($"Категория с Id {product.CategoryId} не найдена.");

        if (product.SectionId.HasValue)
        {
            var sectionExists = await _context.WarehouseSections
                .AnyAsync(s => s.Id == product.SectionId.Value);
            if (!sectionExists)
                throw new KeyNotFoundException($"Секция склада с Id {product.SectionId} не найдена.");
        }

        if (product.SupplierId.HasValue)
        {
            var supplierExists = await _context.Organizations
                .AnyAsync(o => o.Id == product.SupplierId.Value);
            if (!supplierExists)
                throw new KeyNotFoundException($"Поставщик с Id {product.SupplierId} не найден.");
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }


    public async Task<Product?> UpdateAsync(Guid id, Product product)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return null;

        _context.Entry(existing).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Products.FindAsync(id);
        if (existing == null) return false;

        _context.Products.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> FilterAsync(string? name, Guid? categoryId, Guid? sectionId)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (sectionId.HasValue)
            query = query.Where(p => p.SectionId == sectionId);

        return await query.ToListAsync();
    }
}
