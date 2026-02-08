using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Api.Dto.Orders;
using VideoGameApi.Models.Orders;

namespace VideoGameApi.Api.Controllers.Games
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController(VideoGameDbContext context) : ControllerBase
    {
        private int UserId
        {
            get
            {
                var idValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(idValue))
                    throw new InvalidOperationException("Authenticated user is missing an identifier claim.");

                return int.Parse(idValue);
            }
        }

        // checkout or order a set of cart items
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] int[] cartItemIds)
        {
            var cart = await context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.VideoGame)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (cart == null || !cart.Items.Any())
                return BadRequest("Cart is empty");

            var selectedItems = cart.Items
                .Where(i => cartItemIds.Contains(i.Id))
                .ToList();

            if (!selectedItems.Any())
                return BadRequest("No valid items selected");

            var order = new Order
            {
                UserId = UserId,
                TotalPrice = selectedItems.Sum(i => i.VideoGame.Price * i.Quantity),
                Items = selectedItems.Select(i => new OrderItem
                {
                    SourceCartItemId = i.Id,
                    VideoGameId = i.VideoGameId,
                    GameTitle = i.VideoGame.Title,
                    UnitPrice = i.VideoGame.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            context.Orders.Add(order);
            context.CartItems.RemoveRange(selectedItems);
            await context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                Items = order.Items.Select(oi => new OrderItemResponseDto
                {
                    OrderItemId = oi.Id,
                    SourceCartItemId = oi.SourceCartItemId,
                    VideoGameId = oi.VideoGameId,
                    Title = oi.GameTitle,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.UnitPrice * oi.Quantity
                }).ToList()
            };

            return Ok(new
            {
                message = "Checkout successful",
                order = response
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == UserId)
                .ToListAsync();

            var ordersDto = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalPrice = o.TotalPrice,
                Status = (int)o.Status,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    SourceCartItemId = i.SourceCartItemId,
                    VideoGameId = i.VideoGameId,            // <- mapped
                    GameTitle = i.GameTitle,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    ProductKey = i.ProductKey
                }).ToList()
            });

            return Ok(ordersDto);
        }


    }
}
