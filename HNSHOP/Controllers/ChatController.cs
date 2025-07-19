using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<IActionResult> Index()
        {
            int accountId = GetUserAccountId();
            if (accountId == -1) return Unauthorized();

            if (User.IsInRole("User"))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == accountId);
                if (customer == null) return NotFound();

                var conversations = await _context.Conversations
                    .Include(c => c.Shop).ThenInclude(s => s.Account)
                    .Where(c => c.CustomerId == customer.Id && !c.IsDeletedByCustomer)
                    .ToListAsync();

                ViewBag.Conversations = conversations;
                ViewBag.SenderRole = "Customer";
                ViewBag.SenderId = customer.Id;
            }
            else if (User.IsInRole("Shop"))
            {
                var shop = await _context.Shops.FirstOrDefaultAsync(s => s.AccountId == accountId);
                if (shop == null) return NotFound();

                var conversations = await _context.Conversations
                    .Include(c => c.Customer).ThenInclude(c => c.Account)
                    .Where(c => c.ShopId == shop.Id && !c.IsDeletedByShop)
                    .ToListAsync();

                ViewBag.Conversations = conversations;
                ViewBag.SenderRole = "Shop";
                ViewBag.SenderId = shop.Id;
            }

            return View("Chat");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> WithShop(int shopId)
        {
            int accountId = GetUserAccountId();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == accountId);
            if (customer == null) return NotFound();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ShopId == shopId);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    CustomerId = customer.Id,
                    ShopId = shopId,
                    CreatedAt = DateTime.Now
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            else if (conversation.IsDeletedByCustomer)
            {
                conversation.IsDeletedByCustomer = false;
                _context.Update(conversation);
                await _context.SaveChangesAsync();
            }

            await _context.Messages
                .Where(m => m.ConversationId == conversation.Id && !m.IsRead && m.SenderRole == "Shop")
                .ExecuteUpdateAsync(m => m.SetProperty(x => x.IsRead, true));

            var conversations = await _context.Conversations
                .Include(c => c.Shop).ThenInclude(s => s.Account)
                .Where(c => c.CustomerId == customer.Id && !c.IsDeletedByCustomer)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation.Id)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.Conversations = conversations;
            ViewBag.Messages = messages;
            ViewBag.ConversationId = conversation.Id;
            ViewBag.SenderRole = "Customer";
            ViewBag.SenderId = customer.Id;

            return View("Chat");
        }

        [Authorize(Roles = "Shop")]
        public async Task<IActionResult> WithCustomer(int customerId)
        {
            int accountId = GetUserAccountId();
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.AccountId == accountId);
            if (shop == null) return NotFound();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ShopId == shop.Id);

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    CustomerId = customerId,
                    ShopId = shop.Id,
                    CreatedAt = DateTime.Now
                };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
            }
            else if (conversation.IsDeletedByShop)
            {
                conversation.IsDeletedByShop = false;
                _context.Update(conversation);
                await _context.SaveChangesAsync();
            }

            await _context.Messages
                .Where(m => m.ConversationId == conversation.Id && !m.IsRead && m.SenderRole == "Customer")
                .ExecuteUpdateAsync(m => m.SetProperty(x => x.IsRead, true));

            var conversations = await _context.Conversations
                .Include(c => c.Customer).ThenInclude(s => s.Account)
                .Where(c => c.ShopId == shop.Id && !c.IsDeletedByShop)
                .ToListAsync();

            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversation.Id)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.Conversations = conversations;
            ViewBag.Messages = messages;
            ViewBag.ConversationId = conversation.Id;
            ViewBag.SenderRole = "Shop";
            ViewBag.SenderId = shop.Id;

            return View("Chat");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConversation(int conversationId)
        {
            int accountId = GetUserAccountId();
            var convo = await _context.Conversations.FindAsync(conversationId);
            if (convo == null) return NotFound();

            if (User.IsInRole("User"))
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == accountId);
                if (customer == null || convo.CustomerId != customer.Id) return Forbid();
                convo.IsDeletedByCustomer = true;
            }
            else if (User.IsInRole("Shop"))
            {
                var shop = await _context.Shops.FirstOrDefaultAsync(s => s.AccountId == accountId);
                if (shop == null || convo.ShopId != shop.Id) return Forbid();
                convo.IsDeletedByShop = true;
            }

            _context.Update(convo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File trống");

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "chat");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = Url.Content($"~/images/chat/{fileName}");
            return Ok(url);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadMessageCount()
        {
            var userId = GetUserAccountId();
            var role = GetRole(); // "Customer" hoặc "Shop"

            var count = await _context.Conversations
                .Include(c => c.Messages)
                .Where(c =>
                    (role == "Customer" && c.Customer.AccountId == userId) ||
                    (role == "Shop" && c.Shop.AccountId == userId))
                .SelectMany(c => c.Messages)
                .Where(m => !m.IsRead && m.SenderId != userId && m.SenderRole != role)
                .CountAsync();

            return Json(new { count });
        }

        private int GetUserAccountId()
        {
            return int.TryParse(User.FindFirst("AccountId")?.Value
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out int id) ? id : -1;
        }

        private string GetRole()
        {
            if (User.IsInRole("User")) return "Customer";
            if (User.IsInRole("Shop")) return "Shop";
            return string.Empty;
        }

        [HttpPost]
        public IActionResult MarkAsRead(int conversationId)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.IsInRole("User") ? "Customer" : User.IsInRole("Shop") ? "Shop" : null;

            if (string.IsNullOrEmpty(userIdStr) || userRole == null)
                return BadRequest();

            int userId = int.Parse(userIdStr);

            var unreadMessages = _context.Messages
                .Where(m => m.ConversationId == conversationId &&
                            !m.IsRead &&
                            m.SenderRole != userRole &&
                            m.SenderId != userId)
                .ToList();

            if (unreadMessages.Count == 0)
                return Ok(); // Không có gì để cập nhật

            foreach (var msg in unreadMessages)
            {
                msg.IsRead = true;
            }

            _context.SaveChanges();

            return Ok();
        }

    }
}
