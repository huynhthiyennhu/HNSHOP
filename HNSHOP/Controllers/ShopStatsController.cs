using HNSHOP.Data;
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
    }
}
