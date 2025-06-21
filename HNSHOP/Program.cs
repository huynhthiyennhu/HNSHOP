using FluentValidation;
using HNSHOP.Data;
using HNSHOP.Services;
using HNSHOP.Utils;
using HNSHOP.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Database với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//Cấu hình Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

//  Cấu hình Authorization (Phân quyền)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireShop", policy => policy.RequireRole("Shop"));
    options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
});

// FIX LỖI SESSION - Thêm bộ nhớ đệm trước khi gọi Session
builder.Services.AddDistributedMemoryCache(); // Bắt buộc để Session hoạt động
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình MVC với yêu cầu đăng nhập mặc định
builder.Services.AddControllersWithViews();

// Đăng ký IWebHostEnvironment
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);


//Đăng ký các dịch vụ (Dependency Injection)
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICustomerTypeService, CustomerTypeService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<ISaleEventService, SaleEventService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<PayPalConfig>(builder.Configuration.GetSection("PayPal"));
builder.Services.AddScoped<PayPalService>();



//Đăng ký Validator từ FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterShopValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// Cấu hình giới hạn upload file (Fix lỗi tải Avatar)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});


builder.Services.AddAuthorizationBuilder()
     .AddPolicy(ConstConfig.AdminPolicy, policy =>
        policy.RequireRole(ConstConfig.AdminRoleName)
     )
     .AddPolicy(ConstConfig.ShopPolicy, policy =>
         policy.RequireRole(ConstConfig.AdminRoleName, ConstConfig.ShopRoleName)
     )
     .AddPolicy(ConstConfig.UserPolicy, policy =>
          policy.RequireRole(ConstConfig.AdminRoleName, ConstConfig.ShopRoleName, ConstConfig.UserRoleName)
     );

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("ShopPolicy", policy => policy.RequireRole("shop"));
//    options.AddPolicy("UserPolicy", policy => policy.RequireRole("user"));
//});


var app = builder.Build();

//Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Cấu hình định tuyến MVC
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
