using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;
using MyStock.Extensions;
using MyStock.Services;

namespace MyStock.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить список товаров
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Получить товар по Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        /// <summary>
        /// Получить товар по артикулу
        /// </summary>
        [HttpGet("barcode/{barcode}")]
        public async Task<ActionResult<ProductDto>> GetByBarcode(string barcode)
        {
            var product = await _service.GetByBarcodeAsync(barcode);
            return product == null ? NotFound() : Ok(product);
        }

        /// <summary>
        /// Создать товар
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (ArgumentException arg)
            {
                return BadRequest(arg.Message);
            }
        }

        /// <summary>
        /// Обновить товар
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Удалить
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Filter(
        [FromQuery] string? name,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? sectionId)
        {
            return Ok(await _service.FilterAsync(name, categoryId, sectionId));
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
            Unit = p.Unit.ToCodeDisplay(),
            Category = p.Category?.ToRef(),
            Section = p.Section?.ToRef(),
            Supplier = p.Supplier?.ToRef()
        };
    }
}