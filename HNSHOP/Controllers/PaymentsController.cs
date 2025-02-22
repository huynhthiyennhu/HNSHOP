using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Utils.QueryParams;
using HNSHOP.Utils;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = ConstConfig.UserRoleName)]
public class PaymentsController(ApplicationDbContext db, IMapper mapper, IConfiguration config) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _config = config;

    public async Task<IActionResult> Checkout(int orderId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int orderId, string paymentMethod)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return NotFound();

        string vnp_Url = _config["vnp_Url"];
        string vnp_HashSecret = _config["vnp_HashSecret"];
        string vnp_TmnCode = _config["vnp_TmnCode"];
        string vnp_Returnurl = _config["vnp_Returnurl"];

        VnPayLib vnpay = new();
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (order.Total * 1000 * 100).ToString());
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng: {order.Id}");
        vnpay.AddRequestData("vnp_TxnRef", order.Id.ToString());
        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        return Redirect(paymentUrl);
    }

    public async Task<IActionResult> Confirm(VnpayResQuery vnpayResQuery)
    {
        string vnp_HashSecret = _config["vnp_HashSecret"];
        VnPayLib vnpay = new();

        bool isValid = vnpay.ValidateSignature(vnpayResQuery.vnp_SecureHash, vnp_HashSecret);
        if (!isValid) return View(new PaymentResDto { Message = "Invalid signature", ResponseCode = "97" });

        int orderId = int.TryParse(vnpayResQuery.vnp_TxnRef, out int output) ? output : -1;
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return View(new PaymentResDto { Message = "Order not found", ResponseCode = "01" });

        if (vnpayResQuery.vnp_ResponseCode == "00" && vnpayResQuery.vnp_TransactionStatus == "00")
        {
            order.PaymentStatus = PaymentStatus.Completed;
            await _db.SaveChangesAsync();
            return View(new PaymentResDto { Message = "Payment successful", ResponseCode = "00" });
        }

        return View(new PaymentResDto { Message = "Payment failed", ResponseCode = vnpayResQuery.vnp_ResponseCode });
    }
}
