using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace HNSHOP.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmail(string email, string token)
        {
            string subject = "Xác thực tài khoản";
            string body = $"Mã xác thực của bạn là: <b>{token}</b>";
            await SendGeneralEmailAsync(email, subject, body);
        }

        public async Task SendGeneralEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var smtpClient = new SmtpClient(_config["SMTP:Host"])
            {
                Port = int.Parse(_config["SMTP:Port"]),
                Credentials = new NetworkCredential(_config["SMTP:Username"], _config["SMTP:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SMTP:Username"], "HNSHOP"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendApprovedNotification(string email, string shopName, string token)
        {
            string subject = "Shop đã được duyệt - Xác thực tài khoản";
            string body = $@"
        <p>Xin chào,</p>
        <p>Shop <strong>{shopName}</strong> của bạn đã được duyệt.</p>
        <p>Mã xác thực của bạn là: <b>{token}</b></p>
        <p>Vui lòng xác thực để hoàn tất đăng ký.</p>
        <p>Trân trọng,<br/>Hệ thống HNSHOP</p>";
            await SendGeneralEmailAsync(email, subject, body);
        }

        public async Task SendRejectedShopEmail(string email, string shopName)
        {
            string subject = "Tài khoản Shop bị từ chối";
            string body = $@"
        <p>Xin chào,</p>
        <p>Rất tiếc! Shop <strong>{shopName}</strong> của bạn đã bị từ chối đăng ký.</p>
        <p>Vui lòng kiểm tra lại thông tin và đăng ký lại nếu cần.</p>
        <p>Trân trọng,<br/>Hệ thống HNSHOP</p>";
            await SendGeneralEmailAsync(email, subject, body);
        }
        public async Task SendFeePaymentConfirmationEmail(string email, string shopName, string monthLabel, decimal amount)
        {
            string subject = "Xác nhận thanh toán phí bán hàng";

            string body = $@"
        <p>Xin chào <strong>{shopName}</strong>,</p>
        <p>Bạn đã <strong>thanh toán thành công phí bán hàng tháng {monthLabel}</strong>.</p>
        <ul>
            <li><strong>Số tiền:</strong> {amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} VNĐ</li>
            <li><strong>Thời gian:</strong> {DateTime.Now:HH:mm dd/MM/yyyy}</li>
        </ul>
        <p>Cảm ơn bạn đã sử dụng nền tảng HNSHOP.</p>
        <p>Trân trọng,<br/>Hệ thống HNSHOP</p>
    ";

            await SendGeneralEmailAsync(email, subject, body);
        }

    }
}
