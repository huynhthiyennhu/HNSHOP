using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Utils;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HNSHOP.Controllers
{
    [Authorize(Roles = ConstConfig.AdminRoleName)]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminOrdersController> _logger;

        public AdminOrdersController(ApplicationDbContext db, ILogger<AdminOrdersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.Shop)
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.DetailOrders)
                        .ThenInclude(d => d.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _db.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.DetailOrders)
                        .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            if (order.Status != OrderStatus.Processing)
                return BadRequest("Chỉ có thể duyệt đơn hàng đang xử lý.");

            order.Status = OrderStatus.Shipping;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var subOrder in order.SubOrders)
            {
                subOrder.Status = SubOrderStatus.Shipping;

                foreach (var detail in subOrder.DetailOrders)
                {
                    var product = detail.Product;
                    if (product != null)
                    {
                        product.Quantity -= detail.Quantity;

                        // Optional: Ghi nhận chi tiết đơn hàng đã bán
                        product.DetailOrders ??= new List<DetailOrder>();
                        product.DetailOrders.Add(detail);
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã duyệt đơn hàng thành công! Các shop có thể tiến hành giao hàng.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.DetailOrders)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (order.Status != OrderStatus.Cancelled)
                return BadRequest("Chỉ có thể xóa đơn hàng đã bị hủy.");

            foreach (var subOrder in order.SubOrders)
            {
                _db.DetailOrders.RemoveRange(subOrder.DetailOrders);
            }

            _db.SubOrders.RemoveRange(order.SubOrders);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa đơn hàng bị hủy.";
            return RedirectToAction("Index");
        }
    }
}
