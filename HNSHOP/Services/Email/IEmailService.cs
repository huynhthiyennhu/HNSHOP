using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string token);
    }
}
