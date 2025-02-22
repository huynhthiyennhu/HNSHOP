using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IAccountService
    {
        //Task<Account?> GetAccountByIdAsync(int accountId);
        //Task<bool> UpdateAccountAsync(int accountId, UpdateAccountReqDto updateAccountReq);
        Task<Account?> GetByEmailAsync(string email);
        Task<bool> RegisterAsync(RegisterReqDto registerReq);
        Task<bool> RegisterShopAsync(RegisterShopReqDto registerShopReq);
        Task<bool> VerifyEmailAsync(string email, string token);
    }
}
