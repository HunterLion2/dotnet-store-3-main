using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<AppUser> _userManager;
    private SignInManager<AppUser> _signInManager; // Uygulamaya giriş işlemlerini bu parametre ile yaparız.

    public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model) {
        if(ModelState.IsValid){
            // Burada user değeri ile bir kullanıcı oluşturmuş olduk.
            var user = new AppUser {
                UserName = model.Email,
                AsSoyad = model.AdSoyad,
                Email = model.Email
            };

            // CreateAsync değeri bir kullanıcı oluşturmaya yarar ve içine girilen değeri oluşturur.
            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded) {
                return RedirectToAction("Index", "Home");
            }

            foreach(var error in result.Errors) { // Burada yaptığımız şey result.Errors değeri içerisinde çıkan hataları AddModelError değeri ModelState içerisine atmış oluruz error.Description değeri ile de gelen hataları yazdırırız.
                ModelState.AddModelError("",error.Description);
            }

        }
        return View(model);
    }

    public ActionResult Login() {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> LoginAsync(AccountLoginModel model) {
        if(ModelState.IsValid) {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if(user != null) {

                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, model.Password,model.BeniHatirla,false);

                if(result.Succeeded) {
                    return RedirectToAction("Index","Home");
                } else {
                    ModelState.AddModelError("","Hatalı Parola");
                }

            } else {
                ModelState.AddModelError("","Hatalı Email");
            }
        }

        return View(model);
    }

}