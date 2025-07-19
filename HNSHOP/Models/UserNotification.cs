namespace HNSHOP.Models
{
    public class UserNotification
    {
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public bool IsRead { get; set; } = false;
    }



}
