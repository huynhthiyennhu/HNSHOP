using HNSHOP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HNSHOP.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy danh sách thông báo của khách hàng
        /// </summary>
        public async Task<IActionResult> Index()
        {
            int customerId = GetCustomerIdFromToken();
            var notifications = await _notificationService.GetNotificationsAsync(customerId);
            return View(notifications);
        }

        /// <summary>
        /// Đánh dấu thông báo là đã đọc
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            int customerId = GetCustomerIdFromToken();
            await _notificationService.MarkAsReadAsync(customerId, id);
            return RedirectToAction("Index");
        }

        private int GetCustomerIdFromToken()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int customerId) ? customerId : -1;
        }
    }
}
