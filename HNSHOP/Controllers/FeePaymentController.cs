using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;
using System.Security.Claims;

namespace HNSHOP.Controllers
{
    [Route("FeePayment")]
    public class FeePaymentController : Controller
    {
        private readonly PayPalService _paypalService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public FeePaymentController(
            PayPalService paypalService,
            INotificationService notificationService,
            IEmailService emailService,
            ApplicationDbContext context)
        {
            _paypalService = paypalService;
            _notificationService = notificationService;
            _emailService = emailService;
            _context = context;
        }



    [HttpPost("Create")]
    public async Task<IActionResult> Create(decimal amount, int month, int year)
    {
        var returnUrl = Url.Action("Success", "FeePayment", null, Request.Scheme);
        var cancelUrl = Url.Action("Cancel", "FeePayment", null, Request.Scheme);

        TempData["FeeMonth"] = month;               // int OK
        TempData["FeeYear"] = year;                // int OK
        TempData["FeeAmount"] = amount.ToString("F2", CultureInfo.InvariantCulture); // ✅ lưu string

        var approvalUrl = await _paypalService.CreateOrder(
            total: amount,
            currency: "USD",
            returnUrl: returnUrl!,
            cancelUrl: cancelUrl!,
            invoiceNote: $"Phí bán hàng tháng {month:D2}/{year}"
        );

        return Redirect(approvalUrl!);
    }
        public async Task<IActionResult> Success()
        {
            if (TempData["FeeMonth"] is int month &&
                TempData["FeeYear"] is int year &&
                TempData["FeeAmount"] is string amountStr &&
                decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount)) // ✅ parse lại
            {
                int shopId = GetCurrentShopId();

                var shop = await _context.Shops
                    .Include(s => s.Account)
                    .FirstOrDefaultAsync(s => s.Id == shopId);

                if (shop == null) return RedirectToAction("Index", "Home");

                await _emailService.SendFeePaymentConfirmationEmail(
                    shop.Account.Email,
                    shop.Name,
                    $"{month:D2}/{year}",  
                    amount                
                );

                // Thông báo hệ thống
                await _notificationService.SendNotificationToAccountAsync(
                    shop.AccountId,
                    $"Thanh toán thành công tháng {month:D2}/{year}",
                    $"Bạn đã thanh toán phí bán hàng tháng {month:D2}/{year} với số tiền {amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} VNĐ.",
                    "FEE_PAID"
                );
                await _notificationService.SendNotificationToAdminsAsync(
                   $"Shop {shop.Name} đã thanh toán phí",
                   $"Shop {shop.Name} đã thanh toán phí bán hàng tháng {month:D2}/{year} với số tiền {amount:N0} VNĐ.",
                   "FEE_RECEIPT"
               );

                ViewBag.MonthLabel = $"{month:D2}/{year}";
                ViewBag.Amount = amount;
                return View();
            }

            return RedirectToAction("Index", "FeePayment");
        }

       
        [HttpGet("Cancel")]
        public IActionResult Cancel()
        {
            return View("Cancel");
        }

        private int GetCurrentShopId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var shop = _context.Shops.FirstOrDefault(s => s.AccountId == int.Parse(userId));
            if (shop == null)
                throw new Exception("Không tìm thấy thông tin Shop.");
            return shop.Id;
        }
    }
}
