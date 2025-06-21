using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly CartService _cartService;


        public OrderService(ApplicationDbContext db, IMapper mapper, CartService cartService)
        {
            _db = db;
            _mapper = mapper;
            _cartService = cartService;

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
        public async Task<Order> CreateOrderAsync(CreateOrderReqDto orderRequest, int userId)
        {
            var customer = await _db.Customers.Include(c => c.Account).FirstOrDefaultAsync(c => c.AccountId == userId);
            if (customer == null) throw new Exception("Không tìm thấy khách hàng.");

            Address address;
            if (!string.IsNullOrWhiteSpace(orderRequest.NewAddress))
            {
                var newAddress = JsonConvert.DeserializeObject<AddressReqDto>(orderRequest.NewAddress!);
                if (newAddress == null || string.IsNullOrWhiteSpace(newAddress.HouseNumber) ||
                    string.IsNullOrWhiteSpace(newAddress.Street) || string.IsNullOrWhiteSpace(newAddress.Ward) ||
                    string.IsNullOrWhiteSpace(newAddress.District) || string.IsNullOrWhiteSpace(newAddress.City))
                {
                    throw new Exception("Địa chỉ không hợp lệ.");
                }

                string addressDetail = $"{newAddress.HouseNumber}, {newAddress.Street}, {newAddress.Ward}, {newAddress.District}, {newAddress.City}";
                address = new Address { CustomerId = customer.Id, AddressDetail = addressDetail };
                _db.Addresses.Add(address);
                await _db.SaveChangesAsync();
            }
            else
            {
                address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == orderRequest.AddressId && a.CustomerId == customer.Id)
                          ?? throw new Exception("Không tìm thấy địa chỉ đã chọn.");
            }

            var cartItems = _cartService.GetCartItems();
            if (cartItems == null || !cartItems.Any()) throw new Exception("Giỏ hàng trống.");

            var products = await _db.Products.Include(p => p.ProductSaleEvents).ToListAsync();
            decimal totalOrder = 0;
            var orderDetails = new List<DetailOrder>();

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                var discount = await _db.SaleEvents
                    .Where(se => product.ProductSaleEvents.Select(pse => pse.SaleEventId).Contains(se.Id)
                                 && se.StartDate <= DateTime.UtcNow && se.EndDate >= DateTime.UtcNow)
                    .Select(se => se.Discount)
                    .FirstOrDefaultAsync();

                var finalPrice = product.Price * (1 - (decimal)discount / 100);
                totalOrder += finalPrice * item.Quantity;

                orderDetails.Add(new DetailOrder
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = finalPrice
                });
            }

            var order = new Order
            {
                CustomerId = customer.Id,
                AddressId = address.Id,
                Total = totalOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                PaymentStatus = PaymentStatus.Pending,
                DetailOrders = orderDetails
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _cartService.ClearCart();

            return order;
        }
    }
}
