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
            var smtpClient = new SmtpClient(_config["SMTP:Host"])
            {
                Port = int.Parse(_config["SMTP:Port"]),
                Credentials = new NetworkCredential(_config["SMTP:Username"], _config["SMTP:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["SMTP:Username"]),
                Subject = "Xác thực tài khoản",
                Body = $"Mã xác thực của bạn là: <b>{token}</b>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
