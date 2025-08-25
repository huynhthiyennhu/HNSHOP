using HNSHOP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HNSHOP.Controllers
{

    public class AdminStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Tổng số sản phẩm
            ViewBag.TotalProducts = _context.Products.Count();

            // Sản phẩm theo danh mục
            ViewBag.ProductByCategory = _context.Products
                .Include(p => p.ProductType)
                .GroupBy(p => p.ProductType.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToList<object>();

            // Sản phẩm theo shop
            ViewBag.ProductByShop = _context.Products
                .Include(p => p.Shop)
                .GroupBy(p => p.Shop.Name)
                .Select(g => new { ShopName = g.Key, Count = g.Count() })
                .ToList<object>();

            // Top bán chạy
            ViewBag.TopSellingProducts = _context.DetailOrders
                .Include(d => d.Product)
                .GroupBy(d => d.Product.Name)
                .Select(g => new { ProductName = g.Key, Sold = g.Sum(x => x.Quantity) })
                .OrderByDescending(g => g.Sold)
                .Take(5)
                .ToList<object>();

            // Tồn kho thấp
            ViewBag.LowStockProducts = _context.Products
                .Include(p => p.Shop)
                .Where(p => p.Quantity < 10)
                .ToList();

            // Sản phẩm bị ẩn / hết hạn (SaleEvent.EndDate < Now)
            ViewBag.HiddenOrExpiredProducts = _context.Products
                .Include(p => p.ProductSaleEvents)
                    .ThenInclude(pe => pe.SaleEvent)
                .Include(p => p.ProductType)
                .Include(p => p.Shop)
                .Where(p => p.IsDeleted || (p.Quantity==0))
                .ToList();

            return View();
        }
    }
}