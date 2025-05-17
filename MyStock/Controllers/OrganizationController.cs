using MyStock.Entities;
using MyStock.Services;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;
using MyStock.Extensions;

namespace MyStock.Controllers {

    [Route("api/orgs")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _service;
        
        public OrganizationController(OrganizationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все организации
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        /// <summary>
        /// Получить организацию по Id
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrganizationDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<OrganizationDto>>> Filter([FromQuery] OrganizationType type)
        {
            return Ok(await _service.FilterByTypeAsync(type));
        }

        [HttpPost]
        public async Task<ActionResult<OrganizationDto>> Create([FromBody] CreateOrganizationDto dto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrganizationDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }

}
