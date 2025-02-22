using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Utils.QueryParams;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IPaymentService
    {
        Task<string?> CreatePaymentUrlAsync(int orderId, string paymentMethod);
        Task<PaymentResDto> ConfirmPaymentAsync(VnpayResQuery vnpayResQuery);
    }
}
