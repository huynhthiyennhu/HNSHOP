using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly? Dob { get; set; }
        public string Description { get; set; } = string.Empty;

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; } = null!;

        public int CustomerTypeId { get; set; }
        [ForeignKey(nameof(CustomerTypeId))]
        public CustomerType CustomerType { get; set; } = null!;

        public List<Notification> Notifications { get; set; } = [];
        public List<CustomerNotification> CustomerNotifications { get; set; } = [];

        public List<Order> Orders { get; set; } = [];

        public List<Like> Likes { get; set; } = [];

        public List<Rating> Ratings { get; set; } = [];

        public List<Address> Addresses { get; set; } = [];

    }
}
