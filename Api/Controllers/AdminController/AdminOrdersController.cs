using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Models.Orders;
using VideoGameApi.Api.Dto.Orders;

namespace VideoGameApi.Api.Controllers.AdminController
{
    [ApiController]
    [Route("admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController(VideoGameDbContext context) : ControllerBase
    {
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var orders = await context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .Where(o => o.Status == Models.Orders.OrderStatus.Pending)
                .Select(o => new AdminOrderDto
                {
                    Id = o.Id,
                    Username = o.User.Username,
                    Email = o.User.Email,
                    TotalPrice = o.TotalPrice,
                    Status = (Dto.Orders.OrderStatus)o.Status, 
                    CreatedAt = o.CreatedAt,
                    Items = o.Items.Select(i => new AdminOrderItemDto
                    {
                        Id = i.Id,
                        GameTitle = i.GameTitle,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        ProductKey = i.ProductKey
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }


        [HttpPost("{orderId}/approve")]
        public async Task<IActionResult> ApproveOrder(
            int orderId,
            [FromBody] AdminApproveOrderDto dto)
        {
            var order = await context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Order not found");

            if (order.Status != Models.Orders.OrderStatus.Pending)
                return BadRequest("Order already processed");

            foreach (var item in dto.Items)
            {
                var orderItem = order.Items.FirstOrDefault(i => i.Id == item.OrderItemId);
                if (orderItem == null)
                    return BadRequest($"OrderItem {item.OrderItemId} not found");

                orderItem.ProductKey = item.ProductKey;
            }

            order.Status = Models.Orders.OrderStatus.Approved;
            order.ApprovedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(new { message = "Order approved successfully" });
        }

        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound();

            order.Status = Models.Orders.OrderStatus.Rejected;
            await context.SaveChangesAsync();

            return Ok(new { message = "Order rejected" });
        }
    }
}
