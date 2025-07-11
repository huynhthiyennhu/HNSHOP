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
                            .ThenInclude(p => p.ProductSaleEvents)
                                .ThenInclude(pse => pse.SaleEvent)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (order.Status != OrderStatus.Processing)
                return BadRequest("Chỉ có thể duyệt đơn hàng đang xử lý.");

            order.Status = OrderStatus.Preparing;
            order.UpdatedAt = DateTime.UtcNow;

            decimal orderTotal = 0;

            foreach (var subOrder in order.SubOrders)
            {
                subOrder.Status = SubOrderStatus.Pending;
                decimal subTotal = 0;

                foreach (var detail in subOrder.DetailOrders)
                {
                    var product = detail.Product;
                    if (product == null) continue;

                    // Tìm sự kiện giảm giá đang áp dụng
                    var activeSaleEvent = product.ProductSaleEvents?
                        .Select(pse => pse.SaleEvent)
                        .FirstOrDefault(se => se.StartDate <= DateTime.UtcNow && se.EndDate >= DateTime.UtcNow);

                    float discount = activeSaleEvent?.Discount ?? 0f;
                    decimal unitPrice = product.Price;
                    decimal finalPrice = unitPrice * (1 - (decimal)discount / 100);

                    // Lưu giá và giảm giá vào chi tiết đơn hàng
                    detail.UnitPrice = unitPrice;
                    detail.DiscountPercent = discount;

                    // Trừ kho
                    product.Quantity -= detail.Quantity;

                    // Tính tiền
                    decimal itemTotal = finalPrice * detail.Quantity;
                    subTotal += itemTotal;

                    // Optional: liên kết đơn hàng lại (nếu cần)
                    product.DetailOrders ??= new List<DetailOrder>();
                    product.DetailOrders.Add(detail);
                }

                subOrder.SubTotal = subTotal;
                subOrder.Total = subTotal; // Nếu không có thêm phí vận chuyển riêng
                orderTotal += subTotal;
            }

            order.Total = orderTotal;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã duyệt đơn hàng thành công! Các shop có thể tiến hành chuẩn bị giao hàng.";
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
