namespace HNSHOP.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderRole { get; set; } 
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;

        public Conversation Conversation { get; set; }
    }

}
