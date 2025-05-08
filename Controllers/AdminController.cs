using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;


[Authorize(Roles = "Admin")] // Ben bu değer ile admincontroller değerini sadece Roles değeri Admin değerine sahip olanlar girebilir derim

public class AdminController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}