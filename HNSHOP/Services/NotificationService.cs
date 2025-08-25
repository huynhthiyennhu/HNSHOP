using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Services;
using Microsoft.EntityFrameworkCore;

namespace HNSHOP.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendNotificationToAccountAsync(int accountId, string title, string body, string? type = null)
        {
            var notification = new Notification
            {
                Title = title,
                Body = body,
                CreatedAt = DateTime.Now,
                Type = type
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var userNotification = new UserNotification
            {
                AccountId = accountId,
                NotificationId = notification.Id,
                IsRead = false
            };

            _context.UserNotifications.Add(userNotification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDTO>> GetNotificationsAsync(int accountId)
        {
            return await _context.UserNotifications
                .Include(un => un.Notification)
                .Where(un => un.AccountId == accountId)
                .OrderByDescending(un => un.Notification.CreatedAt)
                .Select(un => new NotificationDTO
                {
                    Id = un.Notification.Id,
                    Title = un.Notification.Title,
                    Body = un.Notification.Body,
                    CreatedAt = un.Notification.CreatedAt,
                    IsRead = un.IsRead
                })
                .ToListAsync();
        }


        public async Task MarkAsReadAsync(int accountId, int notificationId)
        {
            var un = await _context.UserNotifications
                .FirstOrDefaultAsync(u => u.AccountId == accountId && u.NotificationId == notificationId);

            if (un != null && !un.IsRead)
            {
                un.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendNotificationToAdminsAsync(string title, string body, string type)
        {
            // Tạo 1 notification chung
            var notification = new Notification
            {
                Title = title,
                Body = body,
                Type = type,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(); // Lưu để có Id

            // Gán notification đó cho tất cả admin
            var adminAccounts = await _context.Accounts
                .Where(a => a.RoleId == 1 && a.IsVerified)
                .ToListAsync();

            foreach (var admin in adminAccounts)
            {
                var userNotification = new UserNotification
                {
                    AccountId = admin.Id,
                    NotificationId = notification.Id,
                    IsRead = false
                };
                _context.Set<UserNotification>().Add(userNotification);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int accountId, int notificationId)
        {
            var userNoti = await _context.UserNotifications
                .FirstOrDefaultAsync(un => un.AccountId == accountId && un.NotificationId == notificationId);

            if (userNoti != null && userNoti.IsRead)
            {
                _context.UserNotifications.Remove(userNoti);

               
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteAllReadNotificationsAsync(int accountId)
        {
            var readNotis = _context.UserNotifications
                .Where(x => x.AccountId == accountId && x.IsRead);

            var count = await readNotis.CountAsync();

            if (count > 0)
            {
                _context.UserNotifications.RemoveRange(readNotis);
                await _context.SaveChangesAsync();
            }

            return count;
        }



    }
}