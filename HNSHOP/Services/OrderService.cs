using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<OrderResDto>> GetAllCustomerOrdersAsync(int customerId)
        {
            var orders = await _db.Orders
                .Include(o => o.DetailOrders)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            return _mapper.Map<List<OrderResDto>>(orders);
        }

        public async Task<OrderResDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.DetailOrders)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return _mapper.Map<OrderResDto>(order);
        }

        public async Task<bool> UpdateOrderShopAsync(int orderId, UpdateOrderShopReqDto updateOrderReq)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return false;

            order.Status = updateOrderReq.Status;
            order.PaymentStatus = updateOrderReq.PaymentStatus;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null || order.Status != Utils.EnumTypes.OrderStatus.Processing) return false;

            order.Status = Utils.EnumTypes.OrderStatus.Cancelled;

            return await _db.SaveChangesAsync() > 0;
        }
    }
}
