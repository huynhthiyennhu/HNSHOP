using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Utils;
using HNSHOP.Utils.QueryParams;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public PaymentService(ApplicationDbContext db, IConfiguration config, IMapper mapper)
        {
            _db = db;
            _config = config;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo URL thanh toán VNPAY
        /// </summary>
        public async Task<string?> CreatePaymentUrlAsync(int orderId, string paymentMethod)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return null;

            VnPayLib vnpay = new();
            string vnp_Url = _config["vnp_Url"] ?? string.Empty;
            string vnp_HashSecret = _config["vnp_HashSecret"] ?? string.Empty;
            string vnp_TmnCode = _config["vnp_TmnCode"] ?? string.Empty;
            string vnp_Returnurl = _config["vnp_Returnurl"] ?? string.Empty;

            if (string.IsNullOrEmpty(vnp_Url) || string.IsNullOrEmpty(vnp_HashSecret) ||
                string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_Returnurl))
            {
                throw new InvalidOperationException("VNPAY configuration is missing in appsettings.json.");
            }

            vnpay.AddRequestData("vnp_Version", VnPayLib.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(order.Total * 1000 * 100)).ToString());
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng: {order.Id}");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Id.ToString());

            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }

        /// <summary>
        /// Xác nhận thanh toán từ VNPAY
        /// </summary>
        public async Task<PaymentResDto> ConfirmPaymentAsync(VnpayResQuery vnpayResQuery)
        {
            try
            {
                string vnp_HashSecret = _config["vnp_HashSecret"] ?? string.Empty;
                VnPayLib vnpay = new();

                if (string.IsNullOrEmpty(vnp_HashSecret))
                {
                    throw new InvalidOperationException("VNPAY Hash Secret is missing in configuration.");
                }

                bool isValid = vnpay.ValidateSignature(vnpayResQuery.vnp_SecureHash, vnp_HashSecret);
                if (!isValid)
                {
                    return new PaymentResDto { Message = "Invalid signature", ResponseCode = "97" };
                }

                if (!int.TryParse(vnpayResQuery.vnp_TxnRef, out int orderId) || orderId <= 0)
                {
                    return new PaymentResDto { Message = "Invalid order reference", ResponseCode = "98" };
                }

                var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    return new PaymentResDto { Message = "Order not found", ResponseCode = "01" };
                }

                if (vnpayResQuery.vnp_ResponseCode == "00" && vnpayResQuery.vnp_TransactionStatus == "00")
                {
                    order.PaymentStatus = Utils.EnumTypes.PaymentStatus.Completed;
                    await _db.SaveChangesAsync();

                    return new PaymentResDto { Message = "Payment successful", ResponseCode = "00" };
                }

                return new PaymentResDto { Message = "Payment failed", ResponseCode = vnpayResQuery.vnp_ResponseCode };
            }
            catch (Exception ex)
            {
                return new PaymentResDto { Message = $"Internal Server Error: {ex.Message}", ResponseCode = "99" };
            }
        }
    }
}
