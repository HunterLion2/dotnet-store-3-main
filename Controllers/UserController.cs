using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;

[Authorize(Roles = "Admin")] // Ben bu değer ile admincontroller değerini sadece Roles değeri Admin değerine sahip olanlar girebilir derim
public class UserController : Controller
{
    // Mesela girilen bir password değerini biz string olarak kaydediyoruz fakat bizim 
    // bunları database atarken şifreli şekilde atmamız lazım aşşağıda işlem buna yarıyor. 

    private UserManager<AppUser> _userManager;
    private RoleManager<AppRole> _roleManager;

    public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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

    public async Task<ActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return RedirectToAction("Index");
        }

        ViewBag.Roles = await _roleManager.Roles.Select(i => i.Name).ToListAsync();

        return View(
            new UserEditModel
            {
                AdSoyad = user.AdSoyad,
                Email = user.Email!,
                SelectedRoles = await _userManager.GetRolesAsync(user)
            }
        );
    }

    [HttpPost]
    public async Task<ActionResult> Edit(string id, UserEditModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.Email = model.Email;
                user.AdSoyad = model.AdSoyad;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded && !string.IsNullOrEmpty(model.Password))
                {
                    // Parola Güncelleme
                    await _userManager.RemovePasswordAsync(user); // Burada yaptığımız eğer kullanıcının bir parolası halihazırda bulunuyorsa bunu kaldırır.
                    await _userManager.AddPasswordAsync(user, model.Password); // Burada da kaldırdığımız değere yeni değerler ekleriz.
                }

                if (result.Succeeded)
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    if (model.SelectedRoles != null)
                    {
                        await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    }
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }
}