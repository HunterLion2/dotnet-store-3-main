using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;

public class UserController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<AppUser> _userManager;

    public UserController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public ActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(AppUserCreate model)
    {

        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                AdSoyad = model.AdSoyad
            };

        var completed = await _userManager.CreateAsync(user);

        if (completed.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in completed.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        }

        return View(model);
    }

}