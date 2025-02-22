using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface ISaleEventService
    {
        Task<List<SaleEventResDto>> GetAllSalesAsync();
        Task<SaleEventResDto?> GetSaleEventByIdAsync(int saleEventId);
        Task<bool> CreateSaleEventAsync(CreateSaleEventReqDto saleEventReq);
        Task<bool> UpdateSaleEventAsync(int saleEventId, UpdateSaleEventReqDto updateSaleReq);
        Task<bool> DeleteSaleEventAsync(int saleEventId);
    }
}
