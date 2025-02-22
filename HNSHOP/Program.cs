using FluentValidation;
using HNSHOP.Data;
using HNSHOP.Services;
using HNSHOP.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Database với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Cấu hình Authentication bằng Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Cấu hình Authorization (Phân quyền)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireShop", policy => policy.RequireRole("Shop"));
    options.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
});

// Cấu hình MVC với yêu cầu đăng nhập mặc định
builder.Services.AddControllersWithViews();

// Đăng ký các dịch vụ (Dependency Injection)
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICustomerTypeService, CustomerTypeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTypeService, ProductTypeService>();
builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<ISaleEventService, SaleEventService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterShopValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

var app = builder.Build();
// Sử dụng Session
//app.UseSession();
// Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

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
