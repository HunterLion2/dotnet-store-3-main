using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager; // Uygulamaya giriş işlemlerini bu parametre ile yaparız.
    private IEmailService _emailService;
    private readonly DataContext _context;

    // ------------- Constructor ----------------
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, DataContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _context = context;
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model)
    {
        if (ModelState.IsValid)
        {
            // Burada user değeri ile bir kullanıcı oluşturmuş olduk.
            var user = new AppUser
            {
                UserName = model.Email,
                AdSoyad = model.AdSoyad,
                Email = model.Email
            };

            // CreateAsync değeri bir kullanıcı oluşturmaya yarar ve içine girilen değeri oluşturur.
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            { // Burada yaptığımız şey result.Errors değeri içerisinde çıkan hataları AddModelError değeri ModelState içerisine atmış oluruz error.Description değeri ile de gelen hataları yazdırırız.
                ModelState.AddModelError("", error.Description);
            }

        }
        return View(model);
    }

    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {

                // _signInManager değeri ile uygulamaya giriş ve çıkışları kontrol ederiz
                await _signInManager.SignOutAsync();

                // PasswordSignInAsync değeri ile bu değerin içine girilen değerler datadakiler uyumuna bakılır.
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.BeniHatirla, true);

                if (result.Succeeded)
                {

                    await TransferCartToUser(user);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (result.IsLockedOut)
                {
                    // GetLockoutEndDateAsync(user) bilgisi kullanıcının giriş için ne kadar sonra hesabının kitlendiği bilgisini bize söyler
                    var locoutDate = await _userManager.GetLockoutEndDateAsync(user);
                    var timeLeft = locoutDate.Value - DateTime.UtcNow;

                    ModelState.AddModelError("", $"Hesabınız Kitlenmiştir.Lütfen {timeLeft.Minutes + 1} dakika sonra tekrar deneyiniz.");
                }
                else
                {
                    ModelState.AddModelError("", "Hatalı Parola");
                }
            }
            else
            {
                ModelState.AddModelError("", "Hatalı Parola");
            }
        }
        else
        {
            ModelState.AddModelError("", "Hatalı Email");
        }
        return View(model);
    }

    private async Task TransferCartToUser(AppUser user)
    {
        await _userManager.ResetAccessFailedCountAsync(user);
        await _userManager.SetLockoutEndDateAsync(user, null);

        var userCart = await _context.Carts.Include(i => i.CartItems)
                           .ThenInclude(i => i.Urun) // Burada yazdığımız ThenInclude değeri , Include ile ulaştığımız diğer birbirine bağlanan değere bağlandıktan sonra o bağlandığımız değerde yine başka bir değer ile bağlı ise ona bağlanmak için yazılır.
                           .Where(i => i.CustomerId == user.UserName)
                           .FirstOrDefaultAsync();

        var cookieCart = await _context.Carts.Include(i => i.CartItems)
                           .ThenInclude(i => i.Urun) 
                           .Where(i => i.CustomerId == Request.Cookies["customerId"])
                           .FirstOrDefaultAsync();

        foreach (var item in cookieCart?.CartItems!)
        {

            var cartItem = userCart?.CartItems.Where(i => i.UrunId == item.UrunId).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Miktar += item.Miktar;
            }
            else
            {
                userCart?.CartItems.Add(new CartItem
                {
                    UrunId = item.UrunId,
                    Miktar = item.Miktar
                });
            }

            _context.Carts.Remove(cookieCart);

            await _context.SaveChangesAsync();
        }

    }

    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [Authorize]
    public ActionResult Settings()
    {
        return View();
    }

    [Authorize]
    public async Task<ActionResult> EditUser()
    {

        // Burada girilen User bilgisi ile kullanıcı bilgilerini almış olduk.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(new AccountEditUserModel
        {
            AdSoyad = user.AdSoyad,
            Email = user.Email!
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> EditUser(AccountEditUserModel model)
    {
        if (ModelState.IsValid)
        {
            // Burada girilen User bilgisi ile kullanıcı bilgilerini almış olduk.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            user.AdSoyad = model.AdSoyad;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Message"] = "Kullanıcı Bilgileriniz Güncellendi";
            }

            foreach (var error in result.Errors)
            { // Burada yaptığımız şey result.Errors değeri içerisinde çıkan hataları AddModelError değeri ModelState içerisine atmış oluruz error.Description değeri ile de gelen hataları yazdırırız.
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    [Authorize]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> ChangePassword(AccountChangePasswordModel model)
    {

        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Parolanız Başarıyla Değiştirildi";
            }

            foreach (var error in result.Errors)
            { // Burada yaptığımız şey result.Errors değeri içerisinde çıkan hataları AddModelError değeri ModelState içerisine atmış oluruz error.Description değeri ile de gelen hataları yazdırırız.
                ModelState.AddModelError("", error.Description);
            }

        }

        return View(model);
    }

    public ActionResult AccessDenied()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            TempData["Message"] = "Email Adresi Boş Geçilemez";
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email); // Burada gönderdiğimiz email adresi ile eşleşen bir email var mı diye bakarız.

        if (user == null)
        {
            TempData["Message"] = "Böyle bir email adresi bulunamadı";
            return View();
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user); // Burada kullanıcı için özel bir token oluşturmuş olduk.

        // Burada bir url oluşturduk çünkü
        // Bu URL, şifre sıfırlama işleminin bir parçasıdır ve kullanıcının güvenli bir şekilde şifresini 
        // sıfırlayabilmesi için gereklidir. URL, kullanıcıyı doğru action'a yönlendirir ve gerekli bilgileri (userId ve token) taşır.
        var url = Url.Action("ResetPassword", "Account", new { userId = user.Id, token });

        var link = $"<a href='http://localhost:5162{url}'>Şifre Sıfırlama Linki</a>";

        await _emailService.SendEmailAsync(user.Email!, "Parola Sıfırlama", link); // Burada email gönderme işlemini yapmış olduk.
        TempData["Message"] = $"Eposta adresinize gönderilen link ile şifreni sıfırlayabilirsin";

        return RedirectToAction("Login");
    }

    public async Task<ActionResult> ResetPassword(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return RedirectToAction("Login");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login");
        }

        var model = new AccountResetPasswordModel
        {
            Token = token,
            Email = user.Email!
        };

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ResetPassword(AccountResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Parolanız Başarıyla Değiştirildi";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }
}