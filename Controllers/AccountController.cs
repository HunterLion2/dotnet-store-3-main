using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager; // Uygulamaya giriş işlemlerini bu parametre ile yaparız.

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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

                    // Buradaki ikilinin ilki ile giriş yaparkenki fail bilgilerini silmiş oluruz. 
                    await _userManager.ResetAccessFailedCountAsync(user);
                    await _userManager.SetLockoutEndDateAsync(user, null);

                    // ---------------------------------------------

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
                ModelState.AddModelError("", "Hatalı Email");
            }
        }

        return View(model);
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

        if(ModelState.IsValid) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if(user == null) {
                return RedirectToAction("Login","Account");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);

            if(result.Succeeded) {
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

}