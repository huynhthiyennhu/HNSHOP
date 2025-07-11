using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HNSHOP.Controllers
{
    public class ShopOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ShopOrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /ShopOrders
        public async Task<IActionResult> Index()
        {
            var shopId = GetCurrentShopId();

            var subOrders = await _db.SubOrders
                .Include(so => so.Order)
                    .ThenInclude(o => o.Customer)
                .Include(so => so.DetailOrders)
                    .ThenInclude(d => d.Product)
                .Where(so => so.ShopId == shopId)
                .OrderByDescending(so => so.CreatedAt)
                .ToListAsync();

            return View(subOrders);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSubOrder(int id)
        {
            var now = DateTime.UtcNow;
            var shopId = GetCurrentShopId();

            var subOrder = await _db.SubOrders
                .Include(so => so.DetailOrders)
                    .ThenInclude(d => d.Product)
                .Include(so => so.Order)
                .FirstOrDefaultAsync(so => so.Id == id && so.ShopId == shopId);

            if (subOrder == null)
                return NotFound("Không tìm thấy đơn hàng của bạn.");

            if (subOrder.Status != SubOrderStatus.Pending)
                return BadRequest("Chỉ có thể duyệt đơn hàng đang chờ xử lý.");

            subOrder.Status = SubOrderStatus.Shipping;

            var order = subOrder.Order;

            var allApproved = await _db.SubOrders
                .Where(so => so.OrderId == order.Id)
                .AllAsync(so => so.Status != SubOrderStatus.Pending);

            if (allApproved && order.Status == OrderStatus.Preparing)
            {
                order.Status = OrderStatus.Shipping;
                order.UpdatedAt = now;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xác nhận giao đơn hàng.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var shopId = GetCurrentShopId();
            var now = DateTime.UtcNow;

            var subOrder = await _db.SubOrders
                .Include(so => so.Order)
                .FirstOrDefaultAsync(so => so.Id == id && so.ShopId == shopId);

            if (subOrder == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (subOrder.Status != SubOrderStatus.Shipping)
                return BadRequest("Chỉ có thể cập nhật đơn đang giao.");

            subOrder.Status = SubOrderStatus.Delivered;
            // subOrder.UpdatedAt = now;

            // Nếu tất cả SubOrder đã Delivered → cập nhật đơn tổng
            var order = subOrder.Order;
            bool allDelivered = await _db.SubOrders
                .Where(so => so.OrderId == order.Id)
                .AllAsync(so => so.Status == SubOrderStatus.Delivered);

            if (allDelivered && order.Status == OrderStatus.Shipping)
            {
                order.Status = OrderStatus.Delivered;
                order.UpdatedAt = now;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xác nhận đơn hàng đã giao.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> CancelSubOrder(int id)
        {
            var shopId = GetCurrentShopId();

            var subOrder = await _db.SubOrders
                .Include(so => so.Order)
                .FirstOrDefaultAsync(so => so.Id == id && so.ShopId == shopId);

            if (subOrder == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (subOrder.Status != SubOrderStatus.Pending)
                return BadRequest("Chỉ được huỷ đơn khi trạng thái đang chờ xử lý.");

            subOrder.Status = SubOrderStatus.Cancelled;

            // Nếu tất cả suborder đều bị huỷ → huỷ luôn đơn tổng
            var order = subOrder.Order;
            var allCancelled = await _db.SubOrders
                .Where(so => so.OrderId == order.Id)
                .AllAsync(so => so.Status == SubOrderStatus.Cancelled);

            if (allCancelled)
            {
                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã huỷ đơn hàng thành công.";
            return RedirectToAction("Index");
        }

        private int GetCurrentShopId()
        {
            var shopClaim = User.FindFirst("ShopId");

            if (shopClaim == null || string.IsNullOrEmpty(shopClaim.Value))
            {
                throw new UnauthorizedAccessException("Tài khoản hiện tại không thuộc về Shop hoặc chưa đăng nhập.");
            }

            if (!int.TryParse(shopClaim.Value, out int shopId))
            {
                throw new Exception("Dữ liệu ShopId không hợp lệ.");
            }

            return shopId;
        }
    }
}