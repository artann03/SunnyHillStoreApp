using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SunnyHillStore.Core.ApplicationDbContexts;

namespace SunnyHillStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AuthorizationConstants.AdminRole)]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("metrics")]
        public async Task<ActionResult<DashboardMetricsDto>> GetDashboardMetrics()
        {
            const int LOW_STOCK_THRESHOLD = 5;

            var totalProducts = await _context.Products.CountAsync(p => !p.IsDeleted);
            var activeUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
            var outOfStockProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.Quantity == 0);
            var lowStockProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.Quantity > 0 && p.Quantity <= LOW_STOCK_THRESHOLD);

            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= thirtyDaysAgo)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;

            var topSellingProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => !oi.IsDeleted)
                .ToListAsync();

            var topProducts = topSellingProducts
                .GroupBy(oi => oi.ProductId)
                .Select(g => new ProductSaleMetric
                {
                    ProductName = g.First().Product.Name,
                    SalesCount = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(5)
                .ToList();

            var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

            var ordersLastSixMonths = await _context.Orders
                .Where(o => o.CreatedAt >= sixMonthsAgo && !o.IsDeleted)
                .ToListAsync();

            var monthlyRevenues = orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenue
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:00}",
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            var metrics = new DashboardMetricsDto
            {
                TotalProducts = totalProducts,
                ActiveUsers = activeUsers,
                OutOfStockProducts = outOfStockProducts,
                LowStockProducts = lowStockProducts,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TopSellingProducts = topProducts,
                MonthlyRevenues = monthlyRevenues
            };

            return Ok(metrics);
        }
    }
} 