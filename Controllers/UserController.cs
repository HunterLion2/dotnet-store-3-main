using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class UserController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public ActionResult Index() {
        var users = _userManager.Users.ToList();
        return View(users);
    }

}