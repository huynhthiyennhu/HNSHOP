namespace HNSHOP.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ShopId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }
        public Shop Shop { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

}
