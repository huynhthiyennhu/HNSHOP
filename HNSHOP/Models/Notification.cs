namespace HNSHOP.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }

}
