using MyStock.Entities;
using MyStock.Services;
using Microsoft.AspNetCore.Mvc;
using MyStock.DTO;

namespace MyStock.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(Guid id)
        {
            var order = await _service.GetByIdAsync(id);
            return order == null
                ? NotFound()
                : Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> Create([FromBody] CreateOrderDto dto)
        {
            var order = await _service.CreateAsync(dto);

            // Преобразуем созданную сущность в DTO для ответа
            var response = new OrderResponseDto
            {
                Id = order.Id,
                Number = order.Number,
                Type = order.Type,
                Status = order.Status,
                ApprovedAt = order.ApprovedAt,
                WarehouseId = order.WarehouseId,
                WarehouseName = string.Empty,       // при необходимости заполнить
                OrganizationId = order.OrganizationId,
                OrganizationName = string.Empty,    // при необходимости заполнить
                ContactId = order.ContactId,
                ContactName = string.Empty,         // при необходимости заполнить
                Comment = order.Comment
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.Id },
                response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrderDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null
                ? NotFound()
                : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted
                ? NoContent()
                : NotFound();
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult<ApprovedOrderDto>> Approve(Guid id)
        {
            var result = await _service.ApproveAsync(id);
            return Ok(result);
        }


        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> Filter(
            [FromQuery] OrderFilterParameters filter)
        {
            var orders = await _service.GetAllAsync();
            var query = orders.AsQueryable();

            if (filter.Status.HasValue)
                query = query.Where(o => o.Status == filter.Status.Value);
            if (filter.WarehouseId.HasValue)
                query = query.Where(o => o.WarehouseId == filter.WarehouseId.Value);
            if (filter.ApprovedFrom.HasValue)
                query = query.Where(o => o.ApprovedAt >= filter.ApprovedFrom.Value);
            if (filter.ApprovedTo.HasValue)
                query = query.Where(o => o.ApprovedAt <= filter.ApprovedTo.Value);

            return Ok(query);
        }
    }

    public class OrderFilterParameters
    {
        public OrderStatus? Status { get; set; }
        public Guid? WarehouseId { get; set; }
        public DateTime? ApprovedFrom { get; set; }
        public DateTime? ApprovedTo { get; set; }
    }
}
