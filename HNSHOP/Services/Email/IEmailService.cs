using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string token);
        Task SendGeneralEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendApprovedNotification(string email, string shopName, string token);
        Task SendRejectedShopEmail(string email, string shopName);
        Task SendFeePaymentConfirmationEmail(string email, string shopName, string monthLabel, decimal amount);

    }
}
