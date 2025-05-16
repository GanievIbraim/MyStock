using Microsoft.AspNetCore.Mvc;
using MyStock.Services;
using MyStock.DTO;

namespace MyStock.Controllers
{
    [Route("api/order-items")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly OrderItemService _service;

        public OrderItemController(OrderItemService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> Create([FromBody] CreateOrderItemDto dto)
        {
            var item = await _service.CreateAsync(dto);
            var response = new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                OrderId = item.OrderId,
                OrderName = item.Order.Number,
                Quantity = item.Quantity,
                Price = item.Price,
                SectionId = item.SectionId,
                SectionName = item.Section != null ? item.Section.Code : string.Empty
            };
            return CreatedAtAction(nameof(GetByOrderId), new { id = response.OrderId }, response);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> CreateBulk([FromBody] List<CreateOrderItemDto> dtos)
        {
            var items = await _service.CreateManyAsync(dtos);
            var result = items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                OrderId = item.OrderId,
                OrderName = item.Order.Number,
                Quantity = item.Quantity,
                Price = item.Price,
                SectionId = item.SectionId,
                SectionName = item.Section != null ? item.Section.Code : string.Empty
            });
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderItemDto>> Update(Guid id, [FromBody] CreateOrderItemDto dto)
        {
            var item = await _service.UpdateAsync(id, dto);
            if (item == null)
                return NotFound();

            var response = new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                OrderId = item.OrderId,
                OrderName = item.Order.Number,
                Quantity = item.Quantity,
                Price = item.Price,
                SectionId = item.SectionId,
                SectionName = item.Section != null ? item.Section.Code : string.Empty
            };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(Guid orderId)
        {
            var items = await _service.GetByOrderIdAsync(orderId);
            return Ok(items);
        }
    }
}
