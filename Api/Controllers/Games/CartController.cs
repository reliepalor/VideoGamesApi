using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Models.Carts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using VideoGameApi.Api.Dto.Cart;

namespace VideoGameApi.Api.Controllers.Games
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/cart")]
    public class CartController(VideoGameDbContext context) : ControllerBase
    {
        private readonly VideoGameDbContext _context = context;

        private int UserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.VideoGame)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (cart == null)
            {
                return Ok(new
                {
                   message = "Your cart is empty",
                   order = cart
                });
            }

            var total = cart.Items.Sum(i => i.Quantity * i.VideoGame.Price);

            return Ok(new
            {
                items = cart.Items.Select(i => new
                {
                    
                    cartItemId = i.Id,                
                    videoGameId = i.VideoGameId,

                    title = i.VideoGame.Title,
                    price = i.VideoGame.Price,
                    quantity = i.Quantity,
                    subtotal = i.Quantity * i.VideoGame.Price,
                    imageUrl = i.VideoGame.ImagePath
                }),
                totalPrice = total
            });
        }



        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            var videoGame = await _context.VideoGames
                .FirstOrDefaultAsync(v => v.Id == dto.VideoGameId);

            if (videoGame == null)
                return BadRequest($"VideoGame with ID {dto.VideoGameId} does not exist.");

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (cart == null)
            {
                cart = new Cart { UserId = UserId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var item = cart.Items
                .FirstOrDefault(i => i.VideoGameId == dto.VideoGameId);

            if (item != null)
            {
                item.Quantity += dto.Quantity; 
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    VideoGameId = dto.VideoGameId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Added to cart",
                videoGameId = dto.VideoGameId,
                quantityAdded = dto.Quantity
            });
        }



        [HttpDelete("remove/{gameId}")]
        public async Task<IActionResult> Remove(int gameId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            if (cart == null) return NotFound();

            var item = cart.Items.FirstOrDefault(i => i.VideoGameId == gameId);
            if (item == null) return NotFound();

            cart.Items.Remove(item);
            _context.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully remove cart item." });
        }
    }
}
