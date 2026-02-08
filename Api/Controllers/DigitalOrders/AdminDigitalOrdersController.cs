using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VideoGameApi.Api.Interfaces.DigitalOrders;

namespace VideoGameApi.Controllers.DigitalOrders
{
    [ApiController]
    [Route("api/admin/digital-orders")]
    [Authorize(Roles = "Admin")]
    public class AdminDigitalOrdersController : ControllerBase
    {
        private readonly IDigitalOrderService _orderService;

        public AdminDigitalOrdersController(IDigitalOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveOrder(int id)
        {
            var result = await _orderService.ApproveOrderAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Order approved" });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectOrder(int id)
        {
            var result = await _orderService.RejectOrderAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Order rejected" });
        }
    }
}
