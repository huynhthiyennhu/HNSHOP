using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Services;
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
        private readonly INotificationService _notificationService;


        public AdminOrdersController(ApplicationDbContext db, ILogger<AdminOrdersController> logger, INotificationService notificationService)
        {
            _db = db;
            _logger = logger;
            _notificationService = notificationService;
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
                subOrder.Total = subTotal; 
                orderTotal += subTotal;
            }

            order.Total = orderTotal;

            await _db.SaveChangesAsync();

            // Gửi thông báo đến khách hàng
            var customer = await _db.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Id == order.CustomerId);

            if (customer != null && customer.Account != null)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: customer.Account.Id,
                    title: "Đơn hàng đã được duyệt",
                    body: $"Đơn hàng #{order.Id} của bạn đã được duyệt và đang được các shop chuẩn bị giao.",
                    type: "Order"
                );
            }

            // Gửi thông báo đến từng shop có SubOrder
            var shopAccounts = await _db.SubOrders
                .Where(so => so.OrderId == order.Id)
                .Include(so => so.Shop)
                    .ThenInclude(s => s.Account)
                .Select(so => so.Shop.Account)
                .Distinct()
                .ToListAsync();

            foreach (var shopAccount in shopAccounts)
            {
                if (shopAccount != null)
                {
                    await _notificationService.SendNotificationToAccountAsync(
                        accountId: shopAccount.Id,
                        title: "Có đơn hàng mới cần xử lý",
                        body: $"Bạn có đơn hàng mới #{order.Id} vừa được duyệt. Vui lòng chuẩn bị hàng để giao.",
                        type: "Order"
                    );
                }
            }


            TempData["SuccessMessage"] = "Đã duyệt đơn hàng thành công! Các shop có thể tiến hành chuẩn bị giao hàng.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.DetailOrders)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Account)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (order.Status != OrderStatus.Cancelled)
                return BadRequest("Chỉ có thể xóa đơn hàng đã bị hủy.");

            // Gửi thông báo cho khách hàng trước khi xóa
            if (order.Customer != null && order.Customer.Account != null)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: order.Customer.Account.Id,
                    title: "Đơn hàng đã bị xóa khỏi hệ thống",
                    body: $"Đơn hàng #{order.Id} đã bị xóa hoàn toàn do đã bị hủy trước đó.",
                    type: "Order"
                );
            }

            // Gửi thông báo đến các shop nếu cần
            var shopAccounts = order.SubOrders
                .Select(so => so.Shop?.Account)
                .Where(a => a != null)
                .Distinct()
                .ToList();

            foreach (var shopAccount in shopAccounts)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: shopAccount.Id,
                    title: "Đơn hàng đã bị xóa",
                    body: $"Đơn hàng #{order.Id} mà shop bạn tham gia đã bị xóa hoàn toàn khỏi hệ thống.",
                    type: "Order"
                );
            }


            // Xoá chi tiết
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
