using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_store.Controllers;


[Authorize] // Ben bu değer ile admincontroller değerini sadece kimliği doğrulanmış kişilerin çağırabileceğini söylemiş olurum.

public class AdminController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}