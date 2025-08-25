using FluentValidation;
using FluentValidation.AspNetCore;
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
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Database với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Cấu hình Authentication bằng Cookie
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

// MVC: tắt required ngầm & Việt hoá thông điệp ModelBinding
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var p = options.ModelBindingMessageProvider;
    p.SetValueMustNotBeNullAccessor(_ => "Trường này là bắt buộc.");
    p.SetMissingBindRequiredValueAccessor(_ => "Thiếu dữ liệu bắt buộc.");
    p.SetAttemptedValueIsInvalidAccessor((v, f) => $"Giá trị \"{v}\" không hợp lệ cho {f}.");
    p.SetValueIsInvalidAccessor(v => $"Giá trị \"{v}\" không hợp lệ.");
    p.SetUnknownValueIsInvalidAccessor(_ => "Giá trị không hợp lệ.");
    p.SetMissingKeyOrValueAccessor(() => "Thiếu dữ liệu.");
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();
//  FluentValidation: auto validate (server) + client adapters
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Đăng ký IWebHostEnvironment
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// Đăng ký các dịch vụ (Dependency Injection)
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
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<VnPayService>();

builder.Services.Configure<PayPalConfig>(builder.Configuration.GetSection("PayPal"));
builder.Services.AddScoped<PayPalService>();

// Đăng ký Validator từ FluentValidation
// FluentValidation (server + client)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterShopValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateSaleEventValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateSaleEventValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<VerifyEmailValidator>();

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

builder.Services.AddSignalR();

// ❌ (ĐÃ BỎ) builder.Services.AddSession();  // trùng với cấu hình session ở trên

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Middleware
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

    // Đăng ký ChatHub
    endpoints.MapHub<ChatHub>("/chathub");
});

app.Run();
