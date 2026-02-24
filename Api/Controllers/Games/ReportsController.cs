using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Api.Dto.Games;
using VideoGameApi.Api.Dto.BestSeller;
using VideoGameApi.Api.Dto.DigitalProducts;
using VideoGameApi.Models.DigitalOrders;
using VideoGameApi.Api.Dto.Reports;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Api.Controllers.Games
{
    // ADMIN REPORTS
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

        // VIDEO GAME SALES REPORT
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

        // DIGITAL PRODUCT SALES REPORT
        [HttpGet("digital-product-sales")]
        public async Task<IActionResult> GetDigitalProductSales()
        {
            var report = await _context.Set<DigitalOrder>()
                .Include(o => o.DigitalProduct)
                .Where(o => o.Status == "Approved") 
                .GroupBy(o => new
                {
                    o.DigitalProductId,
                    o.DigitalProduct.Name
                })
                .Select(g => new DigitalProductSalesDto
                {
                    DigitalProductId = g.Key.DigitalProductId,
                    Name = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToListAsync();

            return Ok(report);
        }
    }

    // PUBLIC REPORTS FOR USERS
    [ApiController]
    [Route("api/reports")]
    public class PublicReportsController : ControllerBase
    {
        private readonly VideoGameDbContext _context;

        public PublicReportsController(VideoGameDbContext context)
        {
            _context = context;
        }

        // VIDEO GAME BEST SELLERS
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var report = await _context.OrderItems
                .Include(oi => oi.VideoGame)
                .GroupBy(oi => new
                {
                    oi.VideoGameId,
                    oi.VideoGame.Title,
                    oi.VideoGame.ImagePath,
                    oi.VideoGame.Price
                })
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

        // DIGITAL PRODUCT BEST SELLERS
        [HttpGet("digital-best-sellers")]
        public async Task<IActionResult> GetDigitalBestSellers()
        {
            var report = await _context.Set<DigitalOrder>()
                .Include(o => o.DigitalProduct)
                .Where(o => o.Status == "Approved")
                .GroupBy(o => new
                {
                    o.DigitalProductId,
                    o.DigitalProduct.Name,
                    o.DigitalProduct.ImagePath,
                    o.DigitalProduct.Price
                })
                .Select(g => new
                {
                    DigitalProductId = g.Key.DigitalProductId,
                    Name = g.Key.Name,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    ImagePath = g.Key.ImagePath,
                    Price = g.Key.Price
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(6)
                .ToListAsync();

            return Ok(report);
        }

        // DASHBOARD SUMMARY
        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary(
            DateTime? startDate,
            DateTime? endDate)
        {
            var videoQuery = _context.OrderItems
                .Include(o => o.Order)
                .AsQueryable();

            var digitalQuery = _context.Set<DigitalOrder>()
                .Where(o => o.Status == "Approved")
                .AsQueryable();

            if (startDate.HasValue)
            {
                videoQuery = videoQuery.Where(x => x.Order.CreatedAt >= startDate.Value);
                digitalQuery = digitalQuery.Where(x => x.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                videoQuery = videoQuery.Where(x => x.Order.CreatedAt <= endDate.Value);
                digitalQuery = digitalQuery.Where(x => x.CreatedAt <= endDate.Value);
            }

            var totalVideoRevenue = await videoQuery
                .SumAsync(x => x.UnitPrice * x.Quantity);

            var totalDigitalRevenue = await digitalQuery
                .SumAsync(x => x.TotalPrice);

            var totalVideoOrders = await videoQuery
                .Select(x => x.OrderId)
                .Distinct()
                .CountAsync();

            var totalDigitalOrders = await digitalQuery.CountAsync();

            var totalUsers = await _context.Set<User>().CountAsync();

            var summary = new DashboardSummaryDto
            {
                TotalVideoGameRevenue = totalVideoRevenue,
                TotalDigitalRevenue = totalDigitalRevenue,
                TotalVideoGameOrders = totalVideoOrders,
                TotalDigitalOrders = totalDigitalOrders,
                TotalUsers = totalUsers
            };

            return Ok(summary);
        }

        // MONTHLY REVENUE REPORT
        [HttpGet("monthly-revenue")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            var videoData = await _context.OrderItems
                .Include(o => o.Order)
                .GroupBy(o => new
                {
                    o.Order.CreatedAt.Year,
                    o.Order.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .ToListAsync();

            var digitalData = await _context.Set<DigitalOrder>()
                .Where(o => o.Status == "Approved")
                .GroupBy(o => new
                {
                    o.CreatedAt.Year,
                    o.CreatedAt.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToListAsync();

            var result = videoData
                .Select(v => new MonthlyRevenueDto
                {
                    Year = v.Year,
                    Month = v.Month,
                    VideoGameRevenue = v.Revenue,
                    DigitalRevenue = digitalData
                        .FirstOrDefault(d => d.Year == v.Year && d.Month == v.Month)?.Revenue ?? 0
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            return Ok(result);
        }

        // TOP 5 COMBINED PRODUCTS
        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts()
        {
            var videoProducts = await _context.OrderItems
                .Include(o => o.VideoGame)
                .GroupBy(o => o.VideoGame.Title)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    ProductType = "VideoGame",
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .ToListAsync();

            var digitalProducts = await _context.Set<DigitalOrder>()
                .Include(o => o.DigitalProduct)
                .Where(o => o.Status == "Approved")
                .GroupBy(o => o.DigitalProduct.Name)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    ProductType = "DigitalProduct",
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice)
                })
                .ToListAsync();

            var combined = videoProducts
                .Concat(digitalProducts)
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            return Ok(combined);
        }
    }
}