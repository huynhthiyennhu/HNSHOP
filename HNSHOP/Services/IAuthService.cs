using HNSHOP.Dtos.Request;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginReqDto loginReq);
    }
}
