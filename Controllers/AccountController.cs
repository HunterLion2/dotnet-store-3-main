using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class AccountController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<IdentityUser> _userManager;

    public AccountController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AccountCreateModel model) {
        if(ModelState.IsValid){
            // Burada user değeri ile bir kullanıcı oluşturmuş olduk.
            var user = new IdentityUser {
                UserName = model.Username,
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

}