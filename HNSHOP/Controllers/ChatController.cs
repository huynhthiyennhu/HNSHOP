using HNSHOP.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HNSHOP.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> WithShop(int shopId)
        {
            int accountId = GetUserAccountId();
            if (accountId == -1) return Unauthorized();

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.AccountId == accountId);
            if (customer == null) return NotFound();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ShopId == shopId);

            if (conversation == null)
            {
                conversation = new Models.Conversation
                {
                    CustomerId = customer.Id,
                    ShopId = shopId,
                    CreatedAt = DateTime.Now
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }

            var conversations = await _context.Conversations
                .Include(c => c.Shop)
                    .ThenInclude(s => s.Account)
                .Where(c => c.CustomerId == customer.Id)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation.Id)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.Conversations = conversations;
            ViewBag.Messages = messages;
            ViewBag.ConversationId = conversation.Id;
            ViewBag.SenderId = customer.Id;
            ViewBag.SenderRole = "Customer";

            return View("Chat");
        }

        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> WithCustomer(int customerId)
        {
            int accountId = GetUserAccountId();
            if (accountId == -1) return Unauthorized();

            var shop = await _context.Shops
                .FirstOrDefaultAsync(s => s.AccountId == accountId);
            if (shop == null) return NotFound();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ShopId == shop.Id);
            if (conversation == null) return NotFound();

            var conversations = await _context.Conversations
                .Include(c => c.Customer)
                    .ThenInclude(cu => cu.Account)
                .Where(c => c.ShopId == shop.Id)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation.Id)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.Conversations = conversations;
            ViewBag.Messages = messages;
            ViewBag.ConversationId = conversation.Id;
            ViewBag.SenderId = shop.Id;
            ViewBag.SenderRole = "Shop";

            return View("Chat");
        }

        private int GetUserAccountId()
        {
            return int.TryParse(User.FindFirst("AccountId")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int id) ? id : -1;
        }
    }
}
