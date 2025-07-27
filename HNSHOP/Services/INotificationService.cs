using HNSHOP.Dtos.Response;
using HNSHOP.Models;


namespace HNSHOP.Services
{
    public interface INotificationService
    {
        Task SendNotificationToAccountAsync(int accountId, string title, string body, string? type = null);
        Task<List<NotificationDTO>> GetNotificationsAsync(int accountId);
        Task MarkAsReadAsync(int accountId, int notificationId);
        Task SendNotificationToAdminsAsync(string title, string body, string type);
        Task DeleteNotificationAsync(int accountId, int notificationId);
        Task DeleteAllNotificationsAsync(int accountId);
    }


}
