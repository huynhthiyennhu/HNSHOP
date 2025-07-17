using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task JoinConversation(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
            throw new ArgumentException("ConversationId is null or empty");

        Console.WriteLine($"[JoinConversation] ID = {conversationId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }


    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendToConversation(string conversationId, string senderRole, int senderId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        if (!int.TryParse(conversationId, out int convId)) return;

        // Lưu tin nhắn vào database
        var msg = new Message
        {
            ConversationId = convId,
            SenderRole = senderRole,
            SenderId = senderId,
            Content = message,
            SentAt = DateTime.Now
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();

        // Lấy avatar của người gửi
        string avatar = "default.png";
        if (senderRole == "Customer")
        {
            avatar = _context.Conversations
                .Include(c => c.Customer)
                .ThenInclude(c => c.Account)
                .Where(c => c.Id == convId)
                .Select(c => c.Customer.Account.Avatar)
                .FirstOrDefault() ?? "default.png";
        }
        else if (senderRole == "Shop")
        {
            avatar = _context.Conversations
                .Include(c => c.Shop)
                .ThenInclude(s => s.Account)
                .Where(c => c.Id == convId)
                .Select(c => c.Shop.Account.Avatar)
                .FirstOrDefault() ?? "default.png";
        }

        // Truyền thêm sentAt và avatar, client tự xử lý isMine
        await Clients.Group(conversationId)
            .SendAsync("ReceiveMessage", senderRole, senderId, message, msg.SentAt.ToString("o"), avatar, true);
    }

}
