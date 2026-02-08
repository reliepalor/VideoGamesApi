using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideoGameApi.Api.Dto.Reviews;
using VideoGameApi.Api.Dto.Games;
using VideoGameApi.Api.Dto.Orders;
using VideoGameApi.Data;
using VideoGameApi.Api.Dto;
using VideoGameApi.Migrations;
using VideoGameApi.Models.Reviews ;
using ModelsOrderStatus = VideoGameApi.Models.Orders.OrderStatus;


namespace VideoGameApi.Api.Controllers.Games
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController(VideoGameDbContext context) : ControllerBase
    {
        private int UserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto dto)
        {
            if (dto.OrderId == null || dto.OrderId <= 0)
                return BadRequest("OrderId is required.");
            var orderId = dto.OrderId.Value;
            // ✅ Check if user purchased & approved
            var order = await context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.UserId == UserId &&
                    o.Status == ModelsOrderStatus.Approved &&          
                    o.Items.Any(i => i.VideoGameId == dto.VideoGameId));

            if (order == null)
                return BadRequest("Order not found, nto approved, or doest not contain this game.");

            //  Prevent duplicate reviews
            var exists = await context.Reviews
                .AnyAsync(r => r.UserId == UserId &&
                   r.VideoGameId == dto.VideoGameId &&
                   r.OrderId == orderId);

            if (exists)
                return BadRequest("You already reviewed this game.");

            //  Fetch videogame (for title)
            var videoGame = await context.VideoGames
                .FirstOrDefaultAsync(v => v.Id == dto.VideoGameId);

            if (videoGame == null)
                return BadRequest("Video game not found.");

            //  Create review
            var review = new Review
            {
                UserId = UserId,
                VideoGameId = dto.VideoGameId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                OrderId = dto.OrderId.Value
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            // Return enriched response
            return Ok(new ReviewResponseDto
            {
                Id = review.Id,
                VideoGameId = videoGame.Id,
                VideoGameTitle = videoGame.Title,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            });
        }


        [HttpGet("game/{videoGameId}")]
        public async Task<IActionResult> GetReviewsForGame(int videoGameId)
        {
            var game = await context.VideoGames
                .FirstOrDefaultAsync(v => v.Id == videoGameId);

            if (game == null)
                return NotFound("Video game not found");

            var reviews = await context.Reviews
                .Include(r => r.User)
                .Where(r => r.VideoGameId == videoGameId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var response = new GameReviewsResponseDto
            {
                VideoGameId = game.Id,
                VideoGameTitle = game.Title,
                AverageRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.Rating), 2)
                    : 0,
                TotalReviews = reviews.Count,
                Reviews = reviews.Select(r => new ReviewItemDto
                {
                    Id = r.Id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    Username = r.User.Username
                }).ToList()
            };

            return Ok( new 
            { 
                message = "You successfully review the item", 
                data = response
            });
        }

        [Authorize]
        [HttpGet("eligibility")]
        public async Task<IActionResult> GetReviewEligibility()
        {
            var reviewed = await context.Reviews
                .Where(r => r.UserId == UserId)
                .Select(r => new
                {
                    r.OrderId,
                    r.VideoGameId
                })
                    .ToListAsync();

            var orders = await context.Orders
                .Include(o => o.Items)
                .Where(o =>
                    o.UserId == UserId &&
                    o.Status == ModelsOrderStatus.Approved)
                .ToListAsync();

            var result = orders
                .SelectMany(o => o.Items.Select(i => new OrderItemWithReviewDto
                {
                    OrderId = o.Id,
                    VideoGameId = i.VideoGameId,
                    GameTitle = i.GameTitle,
                    HasReviewed = reviewed.Any(r =>
                        r.OrderId == o.Id &&
                        r.VideoGameId == i.VideoGameId)
                }))
                .ToList();
            return Ok(result);
        }
    }
}
