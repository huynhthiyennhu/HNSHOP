using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HNSHOP.Controllers
{
    public class ShopStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int shopId = GetCurrentShopId();

            // Tổng sản phẩm
            ViewBag.TotalProducts = _context.Products.Count(p => p.ShopId == shopId);

            // Sản phẩm theo danh mục
            ViewBag.ProductByCategory = _context.Products
                .Where(p => p.ShopId == shopId)
                .GroupBy(p => p.ProductType.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToList();

            // Sản phẩm bán chạy nhất
            ViewBag.TopSellingProducts = _context.DetailOrders
                .Where(d => d.Product.ShopId == shopId)
                .GroupBy(d => d.Product.Name)
                .Select(g => new { ProductName = g.Key, Sold = g.Sum(d => d.Quantity) })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToList();

            // Tồn kho thấp
            ViewBag.LowStockProducts = _context.Products
                .Include(p => p.Shop)
                .Where(p => p.ShopId == shopId && p.Quantity < 10)
                .ToList();

            // Sản phẩm bị ẩn hoặc hết hàng
            ViewBag.HiddenOrExpiredProducts = _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Shop)
                .Where(p => p.ShopId == shopId && p.Quantity == 0)
                .ToList();

            return View();
        }

        private int GetCurrentShopId()
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdStr, out int accountId))
                throw new UnauthorizedAccessException("Không xác định được tài khoản đăng nhập.");

            var shop = _context.Shops.FirstOrDefault(s => s.AccountId == accountId);

            if (shop == null)
                throw new UnauthorizedAccessException("Tài khoản không thuộc về Shop.");

            return shop.Id;
        }
        public async Task<IActionResult> SalesFees()
        {
            int shopId = GetCurrentShopId();
            var feeStats = await GetMonthlyShopFees(shopId);
            return View(feeStats);
        }

        private async Task<List<MonthlyShopFeeDto>> GetMonthlyShopFees(int shopId)
        {
            // Chỉ lấy các SubOrder đã Delivered/Completed
            var validStatuses = new[] { SubOrderStatus.Delivered, SubOrderStatus.Completed };

            var query = await _context.DetailOrders
                .Where(d => d.SubOrder.ShopId == shopId &&
                            validStatuses.Contains(d.SubOrder.Status))
                .Select(d => new
                {
                    Year = d.SubOrder.CreatedAt.Year,
                    Month = d.SubOrder.CreatedAt.Month,
                    Revenue = d.Quantity * d.UnitPrice
                })
                .ToListAsync();

            // sau khi group theo tháng
            var result = query
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new MonthlyShopFeeDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => x.Revenue),
                    IsPaid = _context.Notifications
                        .Any(n => n.Title.Contains("đã thanh toán phí") &&
                                  n.Body.Contains($"{g.Key.Month:00}/{g.Key.Year}"))
                })
                .ToList();


            return result;
        }
        [HttpGet]
        public async Task<IActionResult> FeeDetails(int month, int year)
        {
            int shopId = GetCurrentShopId();

            var validStatuses = new[] { SubOrderStatus.Delivered, SubOrderStatus.Completed };

            var details = await _context.DetailOrders
                .Where(d =>
                    d.SubOrder.ShopId == shopId &&
                    d.SubOrder.Status != null &&
                    validStatuses.Contains(d.SubOrder.Status) &&
                    d.SubOrder.CreatedAt.Month == month &&
                    d.SubOrder.CreatedAt.Year == year)
                .Select(d => new FeeProductDetailDto
                {
                    ProductName = d.Product.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                })
                .ToListAsync();

            ViewBag.MonthLabel = $"{month:00}/{year}";
            return View("FeeDetails", details);
        }


    }
}
