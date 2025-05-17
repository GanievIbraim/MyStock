using Microsoft.AspNetCore.Mvc;
using MyStock.Dto;
using MyStock.DTO;
using MyStock.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyStock.Controllers
{
    [ApiController]
    [Route("api/warehouse-section")]
    public class WarehouseSectionController : ControllerBase
    {
        private readonly WarehouseSectionService _service;

        public WarehouseSectionController(WarehouseSectionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все секции
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseSectionDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Получить секцию по Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WarehouseSectionDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        /// <summary>
        /// Создать новую секцию склада
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WarehouseSectionDto>> Create([FromBody] CreateWarehouseSectionDto dto)
        {
            try
            {
                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, null);
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
        /// Обновить секцию склада
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateWarehouseSectionDto dto)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                return updated ? NoContent() : NotFound();
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
        /// Удалить секцию склада
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
