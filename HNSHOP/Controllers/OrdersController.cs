
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Services;
using HNSHOP.Utils;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace HNSHOP.Controllers
{
    [Authorize(Roles = ConstConfig.UserRoleName)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly CartService _cartService;
        private readonly PayPalService _paypalService;
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;


        public OrdersController(ApplicationDbContext db, CartService cartService, ILogger<OrdersController> logger, PayPalService payPalService, IOrderService orderService, IEmailService emailService, INotificationService notificationService)
        {
            _db = db;
            _cartService = cartService;
            _logger = logger;
            _paypalService = payPalService;
            _orderService = orderService;
            _emailService = emailService;
            _notificationService = notificationService;
        }

       
        public async Task<IActionResult> Index()
        {
            int userId = GetUserIdFromToken();

            var customer = await _db.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Address)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.SubOrders)
                        .ThenInclude(so => so.Shop)
                .Include(c => c.Orders)
                    .ThenInclude(o => o.SubOrders)
                        .ThenInclude(so => so.DetailOrders)
                            .ThenInclude(d => d.Product)
                                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.AccountId == userId);

            if (customer == null)
                return NotFound("Không tìm thấy thông tin khách hàng.");

            // Lấy danh sách sản phẩm đã được đánh giá bởi người dùng hiện tại
            var ratedProductPairs = await _db.Ratings
                .Where(r => r.CustomerId == customer.Id)
                .Select(r => new { r.ProductId, r.SubOrderId })
                .ToListAsync();

            // Gán ViewBag để dùng trong view kiểm tra đã đánh giá chưa
            ViewBag.RatedProductPairs = ratedProductPairs;

            // Tạo danh sách DTO để truyền qua View
            var orderDtos = customer.Orders.Select(o => new OrderResDto
            {
                Id = o.Id,
                Status = o.Status,
                PaymentStatus = o.PaymentStatus,
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                Address = new AddressResDto
                {
                    Id = o.Address.Id,
                    AddressDetail = o.Address.AddressDetail
                },
                Customer = o.Customer,
                SubOrders = o.SubOrders.Select(so => new SubOrderResDto
                {
                    Id = so.Id,
                    OrderId = so.OrderId,
                    ShopId = so.Shop.Id,
                    ShopName = so.Shop.Name,
                    Status = so.Status,
                    Total = so.Total,
                    DetailOrders = so.DetailOrders.Select(d => new DetailOrderResDto
                    {
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Product = new CompactProductResDto
                        {
                            Id = d.Product.Id,
                            Name = d.Product.Name,
                            Price = d.UnitPrice,
                            Images = d.Product.ProductImages
                                    .Take(1)
                                    .Select(img => new ProductImageResDto
                                    {
                                        Id = img.Id,
                                        Path = img.Path
                                    }).ToList(),

                            IsRated = false
                        }
                    }).ToList()
                }).ToList()
            }).ToList();

            return View(orderDtos);
        }


        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                int userId = GetUserIdFromToken();
                if (userId <= 0)
                {
                    TempData["ErrorMessage"] = "Bạn cần đăng nhập để tiếp tục!";
                    return RedirectToAction("Login", "Account");
                }

                // Lấy danh sách địa chỉ
                var addresses = _db.Addresses
                    .Where(a => a.Customer.AccountId == userId)
                    .Select(a => new AddressResDto
                    {
                        Id = a.Id,
                        AddressDetail = a.AddressDetail
                    })
                    .ToList();
                ViewBag.Addresses = addresses ?? new List<AddressResDto>();

                // Lấy giỏ hàng
                var cartItems = _cartService.GetCartItems();
                if (cartItems == null || !cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng trống! Hãy thêm sản phẩm vào giỏ hàng trước khi đặt hàng.";
                    return RedirectToAction("Index", "Home");
                }

                // Gom nhóm theo Shop
                var grouped = cartItems
                    .GroupBy(x => new { x.ShopId, x.ShopName })
                    .Select(g => new ShopCartGroupDto
                    {
                        ShopId = g.Key.ShopId,
                        ShopName = g.Key.ShopName,
                        Items = g.ToList()
                    }).ToList();
                ViewBag.GroupedCartItems = grouped;

                // Tính tổng
                decimal totalOrder = cartItems.Sum(item => item.Total);
                decimal totalDiscount = 0;

                foreach (var item in cartItems)
                {
                    var product = _db.Products.Include(p => p.ProductSaleEvents)
                        .FirstOrDefault(p => p.Id == item.ProductId);

                    if (product != null)
                    {
                        var discount = _db.SaleEvents
                            .Where(se => product.ProductSaleEvents.Select(pse => pse.SaleEventId).Contains(se.Id) &&
                                        se.StartDate <= DateTime.UtcNow && se.EndDate >= DateTime.UtcNow)
                            .Select(se => se.Discount)
                            .FirstOrDefault();

                        totalDiscount += item.Total * (decimal)discount / 100;
                    }
                }

                var orderRequest = new CreateOrderReqDto
                {
                    DetailOrderReqDtos = cartItems.Select(ci => new CreateDetailOrderReqDto
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity
                    }).ToList()
                };

                ViewBag.Total = totalOrder;
                ViewBag.TotalDiscount = totalDiscount;
                ViewBag.FinalTotal = totalOrder - totalDiscount;

                return View(orderRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo trang đặt hàng: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải trang đặt hàng. Vui lòng thử lại!";
                return RedirectToAction("Index", "Home");
            }
        }




        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderReqDto orderRequest)
        {
            try
            {
                if (orderRequest == null || orderRequest.DetailOrderReqDtos == null || !orderRequest.DetailOrderReqDtos.Any())
                    return BadRequest("Dữ liệu đơn hàng không hợp lệ.");

                int userId = GetUserIdFromToken();
                var customer = await _db.Customers.Include(c => c.Account).FirstOrDefaultAsync(c => c.AccountId == userId);
                if (customer == null)
                    return NotFound("Khách hàng không tồn tại.");

                // Xử lý địa chỉ
                Address address;
                if (!string.IsNullOrWhiteSpace(orderRequest.NewAddress))
                {
                    try
                    {
                        var newAddress = JsonConvert.DeserializeObject<AddressReqDto>(orderRequest.NewAddress);
                        if (newAddress == null || string.IsNullOrWhiteSpace(newAddress.PhoneNumber))
                            return BadRequest("Vui lòng nhập đầy đủ thông tin địa chỉ và số điện thoại.");

                        string addressDetail;

                        if (!string.IsNullOrWhiteSpace(newAddress.AddressDetail))
                        {
                            // Nhập toàn bộ địa chỉ bằng tay
                            addressDetail = $"SĐT: {newAddress.PhoneNumber} - {newAddress.AddressDetail}";
                        }
                        else
                        {
                            // Nhập từng phần
                            if (string.IsNullOrWhiteSpace(newAddress.HouseNumber) ||
                                string.IsNullOrWhiteSpace(newAddress.Street) ||
                                string.IsNullOrWhiteSpace(newAddress.Ward) ||
                                string.IsNullOrWhiteSpace(newAddress.District) ||
                                string.IsNullOrWhiteSpace(newAddress.City))
                            {
                                return BadRequest("Vui lòng nhập đầy đủ địa chỉ chi tiết.");
                            }

                            addressDetail = $"SĐT: {newAddress.PhoneNumber} - {newAddress.HouseNumber}, {newAddress.Street}, {newAddress.Ward}, {newAddress.District}, {newAddress.City}";
                        }

                        address = new Address
                        {
                            CustomerId = customer.Id,
                            AddressDetail = addressDetail
                        };

                        _db.Addresses.Add(address);
                        await _db.SaveChangesAsync();
                    }
                    catch (JsonException ex)
                    {
                        return BadRequest($"Lỗi xử lý địa chỉ mới: {ex.Message}");
                    }
                }
                else
                {
                    address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == orderRequest.AddressId && a.CustomerId == customer.Id);
                    if (address == null)
                        return BadRequest("Không tìm thấy địa chỉ đã chọn.");
                }

                // Lấy giỏ hàng
                var cartItems = _cartService.GetCartItems();
                if (cartItems == null || !cartItems.Any())
                    return BadRequest("Giỏ hàng trống!");

                // Lấy sản phẩm
                var products = await _db.Products
                    .Include(p => p.Shop)
                    .Include(p => p.ProductSaleEvents)
                        .ThenInclude(pse => pse.SaleEvent)
                    .ToListAsync();

                // Nhóm sản phẩm theo shop
                var groupedByShop = cartItems.GroupBy(ci =>
                {
                    var product = products.FirstOrDefault(p => p.Id == ci.ProductId);
                    return product?.ShopId ?? 0;
                });

                var subOrders = new List<SubOrder>();
                decimal totalOrder = 0;

                foreach (var group in groupedByShop)
                {
                    int shopId = group.Key;
                    var subOrderDetails = new List<DetailOrder>();
                    decimal subTotal = 0;

                    foreach (var item in group)
                    {
                        var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                        if (product == null) continue;

                        var discount = product.ProductSaleEvents
                            .Where(pse => pse.SaleEvent.StartDate <= DateTime.UtcNow && pse.SaleEvent.EndDate >= DateTime.UtcNow)
                            .Select(pse => pse.SaleEvent.Discount)
                            .FirstOrDefault();

                        var finalPrice = product.Price * (1 - (decimal)discount / 100);
                        subTotal += finalPrice * item.Quantity;

                        subOrderDetails.Add(new DetailOrder
                        {
                            ProductId = product.Id,
                            Quantity = item.Quantity,
                            UnitPrice = finalPrice
                        });
                    }

                    subOrders.Add(new SubOrder
                    {
                        ShopId = shopId,
                        Status = SubOrderStatus.Pending,
                        DetailOrders = subOrderDetails,
                        SubTotal = subTotal,
                        Total = subTotal,
                        CreatedAt = DateTime.UtcNow
                    });

                    totalOrder += subTotal;
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
                    SubOrders = subOrders
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                // Gửi thông báo cho khách hàng
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: customer.Account.Id,
                    title: "Đặt hàng thành công",
                    body: $"Đơn hàng #{order.Id} của bạn đã được đặt thành công và đang chờ duyệt.",
                    type: "Order"
                );

                _cartService.ClearCart();

                TempData["SuccessMessage"] = "Đơn hàng đã được đặt thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xử lý đơn hàng: {ex.Message}");
                return StatusCode(500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
            }
        }




        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            int userId = GetUserIdFromToken();
            var order = await _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.SubOrders)
                    .ThenInclude(so => so.Shop)
                        .ThenInclude(s => s.Account)
                .FirstOrDefaultAsync(o => o.Id == id && o.Customer.AccountId == userId);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");
            if (order.Status != OrderStatus.Processing)
                return BadRequest("Chỉ có thể hủy đơn hàng khi đang xử lý.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            // 🔔 Thông báo khách hàng
            await _notificationService.SendNotificationToAccountAsync(
                accountId: order.Customer.AccountId,
                title: "Bạn đã huỷ đơn hàng",
                body: $"Đơn hàng #{order.Id} đã được bạn huỷ thành công.",
                type: "Order"
            );

            // 🔔 Thông báo các Shop
            foreach (var subOrder in order.SubOrders)
            {
                if (subOrder.Shop?.Account != null)
                {
                    await _notificationService.SendNotificationToAccountAsync(
                        accountId: subOrder.Shop.Account.Id,
                        title: "Đơn hàng bị huỷ",
                        body: $"Đơn hàng #{order.Id} đã bị khách huỷ. Vui lòng kiểm tra lại.",
                        type: "Order"
                    );
                }
            }

            // 🔔 Thông báo Admin
            var adminAccounts = await _db.Accounts
                .Where(a => a.Role.Id == 1) // hoặc dùng RoleId nếu dùng bảng phân quyền riêng
                .ToListAsync();

            foreach (var admin in adminAccounts)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: admin.Id,
                    title: "Đơn hàng bị huỷ bởi khách",
                    body: $"Khách hàng đã huỷ đơn hàng #{order.Id}.",
                    type: "Order"
                );
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateAddress(int id, UpdateOrderCustomerReqDto req)
        {
            int userId = GetUserIdFromToken();
            var order = await _db.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id && o.Customer.AccountId == userId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");
            if (order.Status != OrderStatus.Processing) return BadRequest("Chỉ có thể thay đổi địa chỉ khi đơn hàng đang xử lý.");

            var newAddress = await _db.Addresses.FindAsync(req.AddressId);
            if (newAddress == null) return BadRequest("Địa chỉ không tồn tại.");

            order.AddressId = newAddress.Id;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private int GetUserIdFromToken()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
        }

       
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                int userId = GetUserIdFromToken();

                var order = await _db.Orders
                    .Include(o => o.Address)
                    .Include(o => o.Customer)
                    .Include(o => o.SubOrders)
                        .ThenInclude(so => so.Shop)
                    .Include(o => o.SubOrders)
                        .ThenInclude(so => so.DetailOrders)
                            .ThenInclude(d => d.Product)
                                .ThenInclude(p => p.ProductImages)
                    .FirstOrDefaultAsync(o => o.Id == id && o.Customer.AccountId == userId);

                if (order == null) return NotFound("Không tìm thấy đơn hàng.");

                var orderDto = new OrderResDto
                {
                    Id = order.Id,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    Total = order.Total,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    Address = new AddressResDto
                    {
                        Id = order.Address.Id,
                        AddressDetail = order.Address.AddressDetail
                    },
                    Customer = order.Customer,
                    SubOrders = order.SubOrders.Select(so => new SubOrderResDto
                    {
                        Id = so.Id,
                        OrderId = so.OrderId,
                        ShopId = so.Shop.Id,
                        ShopName = so.Shop.Name,
                        Status = so.Status,
                        DetailOrders = so.DetailOrders.Select(d => new DetailOrderResDto
                        {
                            Quantity = d.Quantity,
                            UnitPrice = d.UnitPrice,
                            Product = new CompactProductResDto
                            {
                                Id = d.Product.Id,
                                Name = d.Product.Name,
                                Price = d.Product.Price,
                                Shop = new ShopResDto
                                {
                                    Id = d.Product.Shop.Id,
                                    Name = d.Product.Shop.Name
                                },
                                DiscountPercent = _db.SaleEvents
                                    .Where(se =>
                                        d.Product.ProductSaleEvents.Select(pse => pse.SaleEventId).Contains(se.Id) &&
                                        se.StartDate <= DateTime.UtcNow && se.EndDate >= DateTime.UtcNow)
                                    .Select(se => se.Discount)
                                    .FirstOrDefault(),

                                Images = d.Product.ProductImages
                                        .Take(1)
                                        .Select(img => new ProductImageResDto
                                        {
                                            Id = img.Id,
                                            Path = img.Path
                                        }).ToList(),


                                // Gợi ý: thêm IsRated nếu cần
                                IsRated = _db.Ratings.Any(r => r.ProductId == d.Product.Id && r.CustomerId == order.Customer.Id)
                            }
                        }).ToList()
                    }).ToList()
                };

                return View(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải chi tiết đơn hàng #{id}: {ex.Message}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải chi tiết đơn hàng. Vui lòng thử lại!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PayWithPaypal(decimal total)
        {
            if (total <= 0)
                return BadRequest("Tổng tiền không hợp lệ.");

            var returnUrl = Url.Action("PaypalSuccess", "Orders", null, Request.Scheme)!;
            var cancelUrl = Url.Action("PaypalCancel", "Orders", null, Request.Scheme)!;

            var paymentUrl = await _paypalService.CreateOrder(total, "USD", returnUrl, cancelUrl);

            return Redirect(paymentUrl ?? "/Orders/Create");
        }

        [HttpPost]
        public IActionResult PreparePaypal([FromBody] CreateOrderReqDto orderRequest)
        {
            if (orderRequest == null || !orderRequest.DetailOrderReqDtos.Any())
            {
                return BadRequest("Dữ liệu đơn hàng không hợp lệ.");
            }

            HttpContext.Session.SetString("PendingOrder", JsonConvert.SerializeObject(orderRequest));

            // Giả định đã có ViewBag.FinalTotal = 123.45 trong Razor, bạn truyền giá trị này vào JS
            return Json(new { redirectUrl = Url.Action("PayWithPaypal", "Orders", new { total = orderRequest.DetailOrderReqDtos.Sum(x => x.Quantity * 1) }) }); // bạn có thể tính lại chính xác ở client
        }
        [HttpGet]
        public async Task<IActionResult> PaypalSuccess(string token)
        {
            var isCaptured = await _paypalService.CaptureOrder(token);
            if (!isCaptured)
            {
                TempData["ErrorMessage"] = "Thanh toán thất bại.";
                return RedirectToAction("Create");
            }

            var orderJson = HttpContext.Session.GetString("PendingOrder");
            if (string.IsNullOrEmpty(orderJson))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Create");
            }

            var orderRequest = JsonConvert.DeserializeObject<CreateOrderReqDto>(orderJson);
            var userId = GetUserIdFromToken();

            var order = await _orderService.CreateOrderAsync(orderRequest!, userId);
            order.PaymentStatus = PaymentStatus.Completed;
            order.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            HttpContext.Session.Remove("PendingOrder");

            var customer = await _db.Customers
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.AccountId == userId);

            if (customer != null && !string.IsNullOrWhiteSpace(customer.Account.Email))
            {
                string email = customer.Account.Email;
                string subject = $"🧾 HNSHOP - Đơn hàng #{order.Id} đã thanh toán thành công!";
                string body = $@"
            <h3>Chào {customer.Name},</h3>
            <p>Chúng tôi xác nhận rằng bạn đã thanh toán thành công đơn hàng <strong>#{order.Id}</strong> qua PayPal.</p>
            <p><b>Tổng tiền:</b> {order.Total.ToString("N0")} VNĐ</p>
            <p>Đơn hàng của bạn sẽ được xử lý và giao hàng trong thời gian sớm nhất.</p>
            <br/>
            <p>Trân trọng,<br/><strong>HNSHOP Team</strong></p>
        ";

                await _emailService.SendGeneralEmailAsync(email, subject, body);
            }

            TempData["SuccessMessage"] = "Thanh toán thành công và đơn hàng đã được ghi nhận!";
            return RedirectToAction("Index");
        }


        public IActionResult PaypalCancel() => View("Cancel");
        public IActionResult PaypalConfirmed() => View("Success");

        [HttpPost]
        public async Task<IActionResult> ConfirmReceived(int id, int subOrderId)
        {
            var userId = GetUserIdFromToken(); // hoặc từ HttpContext

            var customer = await _db.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.SubOrders)
                .FirstOrDefaultAsync(c => c.AccountId == userId);

            if (customer == null)
                return Unauthorized();

            var order = customer.Orders.FirstOrDefault(o => o.Id == id);
            if (order == null)
                return NotFound("Đơn hàng không tồn tại.");

            var subOrder = order.SubOrders.FirstOrDefault(so => so.Id == subOrderId);
            if (subOrder == null || subOrder.Status != SubOrderStatus.Delivered)
                return BadRequest("Đơn hàng không hợp lệ hoặc chưa thể xác nhận.");

            // ✅ Cập nhật trạng thái subOrder
            subOrder.Status = SubOrderStatus.Completed;
            //subOrder.UpdatedAt = DateTime.UtcNow;

            // ✅ Nếu tất cả subOrder đã hoàn tất → order cũng hoàn tất
            bool allCompleted = order.SubOrders.All(so => so.Status == SubOrderStatus.Completed);
            if (allCompleted)
            {
                order.Status = OrderStatus.Completed;
                order.UpdatedAt = DateTime.UtcNow;
            }

            var account = await _db.Accounts.FindAsync(userId);
            if (account != null)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: userId,
                    title: "Xác nhận nhận hàng thành công",
                    body: $"Bạn đã xác nhận đơn hàng #{order.Id} đã được nhận thành công. Cảm ơn bạn!",
                    type: "Order"
                );
            }

            // ✅ Gửi thông báo cho Shop khi khách đã xác nhận nhận hàng
            var shop = await _db.Shops
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.Id == subOrder.ShopId);

            if (shop?.Account != null)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: shop.Account.Id,
                    title: "Khách hàng đã xác nhận nhận hàng",
                    body: $"Khách hàng đã xác nhận đơn hàng #{order.Id} của bạn đã được nhận thành công.",
                    type: "SubOrder"
                );
            }

            var admins = await _db.Accounts.Where(a => a.RoleId == 1).ToListAsync();
            foreach (var admin in admins)
            {
                await _notificationService.SendNotificationToAccountAsync(
                    accountId: admin.Id,
                    title: "Đơn hàng đã hoàn tất",
                    body: $"Đơn hàng #{order.Id} đã được khách xác nhận hoàn tất.",
                    type: "Order"
                );
            }


            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bạn đã xác nhận đã nhận hàng thành công.";
            return RedirectToAction("Index");
        }

    }
}
