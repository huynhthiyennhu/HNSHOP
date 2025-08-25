using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using HNSHOP.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using HNSHOP.Utils;

public class AuthController(ApplicationDbContext db, IEmailService emailService,
    IValidator<RegisterReqDto> registerValidator,
    IValidator<RegisterShopReqDto> registerShopValidator,
    IValidator<LoginReqDto> loginValidator,
    INotificationService notificationService) : Controller
{
    private readonly ApplicationDbContext _db = db;

    private readonly IEmailService _emailService = emailService;
    private readonly IValidator<RegisterReqDto> _registerValidator = registerValidator;
    private readonly IValidator<RegisterShopReqDto> _registerShopValidator = registerShopValidator;
    private readonly IValidator<LoginReqDto> _loginValidator = loginValidator;
    private readonly INotificationService _notificationService = notificationService;


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterReqDto request)
    {
        if (!ModelState.IsValid) return View(request);

        // Chuẩn hóa input (không ảnh hưởng validate, vì validate đã chạy ở trên)
        request.Email = request.Email?.Trim().ToLowerInvariant();
        request.Phone = request.Phone?.Trim();
        request.Name = request.Name?.Trim();

        // Kiểm tra trùng
        if (await _db.Accounts.AnyAsync(a => a.Email == request.Email))
        {
            ModelState.AddModelError("", "Email đã tồn tại.");
            return View(request);
        }
        if (await _db.Accounts.AnyAsync(a => a.Phone == request.Phone))
        {
            ModelState.AddModelError(nameof(request.Phone), "Số điện thoại đã được sử dụng.");
            return View(request);
        }

        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var verifyToken = GenerateVerificationToken();

            var account = new Account
            {
                Email = request.Email,
                Phone = request.Phone,
                Password = hashedPassword,
                RoleId = 2,
                VerifyToken = verifyToken,
                IsVerified = false,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            var customer = new Customer
            {
                Name = request.Name,
                AccountId = account.Id,
                CustomerTypeId = 1,
                Description = "Khách hàng mới"
            };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            // Gửi email xác thực (không chặn flow nếu lỗi)
            try
            {
                await _emailService.SendVerificationEmail(request.Email, verifyToken);
            }
            catch
            {
                TempData["Warning"] = "Tạo tài khoản thành công nhưng gửi email xác thực chưa thành công. Vui lòng thử gửi lại.";
            }

            // Thông báo Admin (không chặn flow nếu lỗi)
            try
            {
                var admins = await _db.Accounts
                    .Where(a => a.RoleId == 1 && a.IsVerified)
                    .ToListAsync();

                foreach (var admin in admins)
                {
                    await _notificationService.SendNotificationToAccountAsync(
                        accountId: admin.Id,
                        title: "Khách hàng mới đăng ký",
                        body: $"Người dùng \"{customer.Name}\" vừa đăng ký tài khoản.",
                        type: "Customer"
                    );
                }
            }
            catch { /* ignore */ }

            await tx.CommitAsync();

            TempData["Message"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác thực.";
            return RedirectToAction("VerifyEmail");
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", "Dữ liệu thay đổi trong lúc xử lý. Vui lòng thử lại.");
            return View(request);
        }
        catch (DbUpdateException)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", "Có lỗi khi lưu dữ liệu. Vui lòng thử lại.");
            return View(request);
        }
        catch (Exception)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", "Đã xảy ra lỗi không xác định. Vui lòng thử lại.");
            return View(request);
        }
    }


    // Hiển thị trang đăng ký Shop
    [HttpGet]
    public IActionResult RegisterShop()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterShop(RegisterShopReqDto request)
    {
        // Nếu dùng FluentValidation Auto, không cần gọi _registerShopValidator.ValidateAsync
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        request.Email = request.Email?.Trim().ToLower();
        request.Phone = request.Phone?.Trim();
        request.Name = request.Name?.Trim();

        // Kiểm tra email đã tồn tại
        if (await _db.Accounts.AnyAsync(a => a.Email == request.Email))
        {
            ModelState.AddModelError(nameof(request.Email), "Email đã tồn tại.");
            return View(request);
        }

        // Kiểm tra số điện thoại đã tồn tại
        if (await _db.Accounts.AnyAsync(a => a.Phone == request.Phone))
        {
            ModelState.AddModelError(nameof(request.Phone), "Số điện thoại đã tồn tại.");
            return View(request);
        }

        // Hash password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var account = new Account
        {
            Email = request.Email,
            Phone = request.Phone,
            Password = hashedPassword,
            RoleId = 3, // Shop
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsApproved = false,
            IsVerified = true // Nếu muốn xác thực qua email thì set false
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        var shop = new Shop
        {
            Name = request.Name,
            AccountId = account.Id,
            Description = "Shop mới đăng ký"
        };
        _db.Shops.Add(shop);
        await _db.SaveChangesAsync();

        await _notificationService.SendNotificationToAdminsAsync(
            title: "Shop mới đăng ký",
            body: $"Shop \"{shop.Name}\" vừa đăng ký và đang chờ duyệt.",
            type: "Shop"
        );

        TempData["Message"] = "Đăng ký shop thành công! Tài khoản đang chờ quản trị viên duyệt.";
        return RedirectToAction("Login");
    }


    // Hiển thị trang xác thực email
    [HttpGet]
    public IActionResult VerifyEmail()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail(string email, string token)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);

        if (account == null)
        {
            ModelState.AddModelError("", "Tài khoản không tồn tại.");
            return View();
        }

        // Nếu là Shop thì bắt buộc phải được duyệt
        if (account.RoleId == 3 && !account.IsApproved)
        {
            ModelState.AddModelError("", "Tài khoản Shop của bạn chưa được quản trị viên duyệt.");
            return View();
        }

        if (account.VerifyToken != token)
        {
            ModelState.AddModelError("", "Mã xác thực không hợp lệ.");
            return View();
        }

        if (account.IsVerified)
        {
            TempData["Message"] = "Tài khoản đã xác thực trước đó.";
            return RedirectToAction("Login");
        }

        account.IsVerified = true;
        account.VerifiedAt = DateTime.Now;
        account.VerifyToken = null;

        await _db.SaveChangesAsync();

        TempData["Message"] = "Xác thực email thành công! Bạn có thể đăng nhập ngay.";
        return RedirectToAction("Login");
    }


    // Hiển thị trang đăng nhập
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginReqDto request, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(request);

        request.Email = request.Email?.Trim().ToLowerInvariant();

        var account = await _db.Accounts
            .Include(a => a.Role)
            .Include(a => a.Customer)
            .Include(a => a.Shop)
            .FirstOrDefaultAsync(a => a.Email.ToLower() == request.Email);

       
        if (account == null)
        {
            _ = BCrypt.Net.BCrypt.Verify(request.Password ?? string.Empty,
                                         "$2a$11$abcdefghijklmnopqrstuvC9tH5B1m0Nw4v0i3hBv2N6x5oG1P2i3");
            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View(request);
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password ?? string.Empty, account.Password))
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View(request);
        }

        if (!account.IsVerified)
        {
            ModelState.AddModelError("", "Tài khoản chưa được xác thực. Vui lòng kiểm tra email.");
            return View(request);
        }

        
        if (account.RoleId == 3 && !account.IsApproved)
        {
            ModelState.AddModelError("", "Tài khoản Shop của bạn đang chờ duyệt.");
            return View(request);
        }

        // Claims
        var roleName = account.Role?.Name ?? ConstConfig.UserRoleName;
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim(ClaimTypes.Email, account.Email),
        new Claim(ClaimTypes.Role, roleName),
        new Claim(ConstConfig.UserIdClaimType, account.Id.ToString()),
        new Claim(ClaimTypes.Name, account.Customer?.Name ?? account.Shop?.Name ?? "Admin"),
        new Claim("Avatar", account.Avatar ?? "default.png")
    };
        if (account.Shop != null)
        {
            claims.Add(new Claim("ShopId", account.Shop.Id.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProps = new AuthenticationProperties
        {
            IsPersistent = true, 
            ExpiresUtc = DateTime.UtcNow.AddMinutes(ConstConfig.ExperyTimeJwtToken),
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            authProps
        );

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    private string GenerateVerificationToken()
    {
        var rng = new Random();
        return rng.Next(100000, 999999).ToString(); 
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        if (account == null)
        {
            ModelState.AddModelError("", "Email không tồn tại.");
            return View();
        }

        string token = new Random().Next(100000, 999999).ToString();
        account.VerifyToken = token;
        await _db.SaveChangesAsync();

        await _emailService.SendGeneralEmailAsync(email, "Khôi phục mật khẩu", $"Mã xác thực của bạn là: <b>{token}</b>");

        TempData["Email"] = email;
        return RedirectToAction("VerifyResetCode");
    }

    [HttpGet]
    public IActionResult VerifyResetCode() => View();

    [HttpPost]
    public async Task<IActionResult> VerifyResetCode(string email, string token, string newPassword)
    {
        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.VerifyToken == token);
        if (account == null)
        {
            ModelState.AddModelError("", "Mã xác thực không hợp lệ.");
            return View();
        }

        account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        account.VerifyToken = null;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Mật khẩu đã được đặt lại.";
        return RedirectToAction("Login");
    }
}
