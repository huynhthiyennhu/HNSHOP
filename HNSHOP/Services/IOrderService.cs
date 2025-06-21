using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IOrderService
    {
        Task<List<OrderResDto>> GetAllCustomerOrdersAsync(int customerId);
        Task<OrderResDto?> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderShopAsync(int orderId, UpdateOrderShopReqDto updateOrderReq);
        Task<bool> CancelOrderAsync(int orderId);
        Task<Order> CreateOrderAsync(CreateOrderReqDto orderRequest, int userId);

    }
}
