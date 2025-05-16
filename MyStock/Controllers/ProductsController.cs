using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;

[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _service.GetAllAsync();
        return Ok(products.Select(p => Map(p)));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(Map(product));
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<ActionResult<ProductDto>> GetByBarcode(string barcode)
    {
        var product = await _service.GetByBarcodeAsync(barcode);
        return product == null ? NotFound() : Ok(Map(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
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
            SupplierId = dto.SupplierId,
            SectionId = dto.SectionId
        };

        var created = await _service.CreateAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, Map(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Code = dto.Code,
            Barcode = dto.Barcode,
            Description = dto.Description,
            Quantity = dto.Quantity,
            Price = dto.Price,
            Unit = dto.Unit,
            CategoryId = dto.CategoryId,
            SupplierId = dto.SupplierId,
            SectionId = dto.SectionId
        };

        var updated = await _service.UpdateAsync(id, product);
        return updated == null ? NotFound() : NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Filter([FromQuery] string? name, [FromQuery] Guid? categoryId, [FromQuery] Guid? sectionId)
    {
        var result = await _service.FilterAsync(name, categoryId, sectionId);
        return Ok(result.Select(p => Map(p)));
    }

    private ProductDto Map(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Code = p.Code,
        Barcode = p.Barcode,
        Description = p.Description,
        Quantity = p.Quantity,
        Price = p.Price,
        Unit = p.Unit,
        CategoryId = p.CategoryId,
        SectionId = p.SectionId,
        SupplierId = p.SupplierId
    };
}
