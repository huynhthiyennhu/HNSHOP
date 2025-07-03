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
    IValidator<LoginReqDto> loginValidator) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IEmailService _emailService = emailService;
    private readonly IValidator<RegisterReqDto> _registerValidator = registerValidator;
    private readonly IValidator<RegisterShopReqDto> _registerShopValidator = registerShopValidator;
    private readonly IValidator<LoginReqDto> _loginValidator = loginValidator;


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
    public async Task<IActionResult> Register(RegisterReqDto request)
    {

        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(request);
        }

        if (await _db.Accounts.AnyAsync(a => a.Email == request.Email))
        {
            ModelState.AddModelError("", "Email đã tồn tại.");
            return View(request);
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        string verifyToken = GenerateVerificationToken();

        var account = new Account
        {
            Email = request.Email,
            Phone = request.Phone,
            Password = hashedPassword,
            RoleId =2,  
            VerifyToken = verifyToken,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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

        await _emailService.SendVerificationEmail(request.Email, verifyToken);

        TempData["Message"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác thực.";
        return RedirectToAction("VerifyEmail");
    }

    // Hiển thị trang đăng ký Shop
    [HttpGet]
    public IActionResult RegisterShop()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> RegisterShop(RegisterShopReqDto request)
    {
        var validationResult = await _registerShopValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(request);
        }

        if (await _db.Accounts.AnyAsync(a => a.Email == request.Email))
        {
            ModelState.AddModelError("", "Email đã tồn tại.");
            return View(request);
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var account = new Account
        {
            Email = request.Email,
            Phone = request.Phone,
            Password = hashedPassword,
            RoleId = 3,  
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
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
    public async Task<IActionResult> Login(LoginReqDto request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(request);
        }

        var account = await _db.Accounts
            .Include(a => a.Role)
            .Include(a => a.Customer)
            .Include(a => a.Shop)
            .FirstOrDefaultAsync(a => a.Email == request.Email);


        if (account == null || !BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
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


        var role = account.Role?.Name ?? ConstConfig.UserRoleName;

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        new Claim(ClaimTypes.Email, account.Email),
        new Claim(ClaimTypes.Role, role), 
        new Claim(ConstConfig.UserIdClaimType, account.Id.ToString()),
        new Claim(ClaimTypes.Name, account.Customer?.Name ?? account.Shop?.Name ?? "Admin"),
        new Claim("Avatar", account.Avatar ?? "default.png") 
    };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddMinutes(ConstConfig.ExperyTimeJwtToken)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

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
