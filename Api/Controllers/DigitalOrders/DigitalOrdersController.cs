using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using VideoGameApi.Api.Dto.DigitalOrders;
using VideoGameApi.Api.Interfaces.DigitalOrders;

namespace VideoGameApi.Controllers.DigitalOrders
{
    [ApiController]
    [Route("api/digital-orders")]
    [Authorize]
    public class DigitalOrdersController : ControllerBase
    {
        private readonly IDigitalOrderService _orderService;

        public DigitalOrdersController(IDigitalOrderService orderService)
        {
            _orderService = orderService;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(CreateDigitalOrderDto dto)
        {
            var orderId = await _orderService.CreateOrderAsync(UserId, dto);
            return Ok(new { message = "Order created", orderId });
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await _orderService.GetUserOrdersAsync(UserId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id, UserId, false);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
