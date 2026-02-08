using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Api.Dto.Games;
using VideoGameApi.Api.Dto.BestSeller;

namespace VideoGameApi.Api.Controllers.Games
{
    [ApiController]
    [Route("api/admin/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly VideoGameDbContext _context;

        public ReportsController(VideoGameDbContext context)
        {
            _context = context;
        }

        // ADMIN: full sales report
        [HttpGet("videogame-sales")]
        public async Task<IActionResult> GetVideoGameSales()
        {
            var report = await _context.OrderItems
                .Include(oi => oi.VideoGame)
                .GroupBy(oi => new { oi.VideoGameId, oi.VideoGame.Title })
                .Select(g => new VideoGameSalesDto
                {
                    VideoGameId = g.Key.VideoGameId,
                    Title = g.Key.Title,
                    TotalNumbers = g.Count(),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToListAsync();

            return Ok(report);
        }
    }

    // PUBLIC: best sellers (no admin role)
    [ApiController]
    [Route("api/reports")]
    public class PublicReportsController : ControllerBase
    {
        private readonly VideoGameDbContext _context;

        public PublicReportsController(VideoGameDbContext context)
        {
            _context = context;
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var report = await _context.OrderItems
                .Include(oi => oi.VideoGame)
                .GroupBy(oi => new { oi.VideoGameId, oi.VideoGame.Title, oi.VideoGame.ImagePath, oi.VideoGame.Price })
                .Select(g => new BestSellerDto
                {
                    VideoGameId = g.Key.VideoGameId,
                    Title = g.Key.Title,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity),
                    ImagePath = g.Key.ImagePath,
                    Price = g.Key.Price
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(6)
                .ToListAsync();

            return Ok(report);
        }
    }
}
