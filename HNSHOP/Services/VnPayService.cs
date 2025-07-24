using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace HNSHOP.Services
{
    public class VnPayService
    {
        private readonly IConfiguration _config;
        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(decimal amount, string orderId, string ipAddress)
        {
            var vnpUrl = _config["VnPay:Url"];
            var returnUrl = _config["VnPay:ReturnUrl"];
            var tmnCode = _config["VnPay:TmnCode"];
            var hashSecret = _config["VnPay:HashSecret"];

            var vnpParams = new SortedDictionary<string, string>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", tmnCode },
        { "vnp_Amount", ((long)(amount * 100)).ToString() },
        { "vnp_CurrCode", "VND" },
        { "vnp_TxnRef", orderId },
        { "vnp_OrderInfo", $"Thanh toán đơn hàng #{orderId}" },
        { "vnp_OrderType", "other" },
        { "vnp_Locale", "vn" },
        { "vnp_IpAddr", ipAddress ?? "127.0.0.1" },
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
        { "vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss") },
        { "vnp_ReturnUrl", returnUrl }
    };

            string rawData = string.Join('&', vnpParams.Select(kv => $"{kv.Key}={kv.Value}"));
            string hash = HmacSHA512(hashSecret, rawData);
            vnpParams.Add("vnp_SecureHash", hash);
            Console.WriteLine("🔍 rawData: " + rawData);
            Console.WriteLine("🔑 SecureHash: " + hash);

            return QueryHelpers.AddQueryString(vnpUrl, vnpParams);
        }

        private static string HmacSHA512(string key, string inputData)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }

    }
}
