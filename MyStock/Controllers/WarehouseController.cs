﻿using MyStock.Services;
using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;

namespace MyStock.Controllers
{
    [ApiController]
    [Route("api/warehouse")]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseService _service;

        public WarehouseController(WarehouseService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить список складов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        /// <summary>
        /// Получить склад по Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetById(Guid id)
        {
            var warehouse = await _service.GetByIdAsync(id);
            return warehouse == null ? NotFound() : Ok(warehouse);
        }

        /// <summary>
        /// Создать новый склад
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateWarehouseDto dto)
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
        /// Обновить склад
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateWarehouseDto dto)
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
        /// Удалить склад
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
