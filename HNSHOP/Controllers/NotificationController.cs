using HNSHOP.Data;
using HNSHOP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private int GetAccountId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    public async Task<IActionResult> Index()
    {
        var accountId = GetAccountId();
        var notifications = await _notificationService.GetNotificationsAsync(accountId);
        return View(notifications); // Notifications là List<NotificationResDTO>
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var accountId = GetAccountId();
        await _notificationService.MarkAsReadAsync(accountId, notificationId);
        return Ok(); // Trả về JSON 200
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int notificationId)
    {
        var accountId = GetAccountId();
        await _notificationService.DeleteNotificationAsync(accountId, notificationId);
        return Ok();
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAll()
    {
        var accountId = GetAccountId();
        var deleted = await _notificationService.DeleteAllReadNotificationsAsync(accountId);
        return Ok(new { success = true, deleted });
    }


}
