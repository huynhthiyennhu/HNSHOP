using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IShopService
    {
        Task<List<ShopResDto>> GetAllShopsAsync();
        Task<ShopResDto?> GetShopByIdAsync(int shopId);
        Task<bool> UpdateShopAsync(int shopId, UpdateShopReqDto updateShopReq);
        Task<bool> DeleteShopAsync(int shopId);
    }
}
