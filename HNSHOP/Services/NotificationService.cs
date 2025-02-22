using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _db;

        public NotificationService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách thông báo của khách hàng
        /// </summary>
        public async Task<List<Notification>> GetNotificationsAsync(int customerId)
        {
            return await _db.Notifications
                .Where(n => n.CustomerNotifications.Any(cn => cn.CustomerId == customerId))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Thêm thông báo mới cho khách hàng
        /// </summary>
        public async Task AddNotificationAsync(int customerId, string title, string body)
        {
            var notification = new Notification
            {
                Title = title,
                Body = body,
                CreatedAt = DateTime.Now,
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            // Thêm vào bảng trung gian
            var customerNotification = new CustomerNotification
            {
                CustomerId = customerId,
                NotificationId = notification.Id,
                IsRead = false
            };

            _db.CustomerNotifications.Add(customerNotification);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Đánh dấu thông báo là đã đọc
        /// </summary>
        public async Task MarkAsReadAsync(int customerId, int notificationId)
        {
            var customerNotification = await _db.CustomerNotifications
                .FirstOrDefaultAsync(cn => cn.CustomerId == customerId && cn.NotificationId == notificationId);

            if (customerNotification != null)
            {
                customerNotification.IsRead = true;
                await _db.SaveChangesAsync();
            }
        }
    }
}
