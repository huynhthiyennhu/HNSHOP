namespace HNSHOP.Models
{
    public class CustomerNotification
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public bool IsRead { get; set; } = false; // Trạng thái đã đọc thông báo hay chưa
    }
}
