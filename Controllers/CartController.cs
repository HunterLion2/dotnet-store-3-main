using System.Threading.Tasks;
using dotnet_store.Models;
using dotnet_store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;


[Authorize] // Ben bu değer ile admincontroller değerini sadece Roles değeri Admin değerine sahip olanlar girebilir derim
public class CartController : Controller
{
    
    private readonly ICartService _cartSerice;

    public CartController(ICartService cartSerice)
    {
        _cartSerice = cartSerice;
    }

    public async Task<ActionResult> Index()
    {
        var customerId = _cartSerice.GetCustomerId();
        var cart = await _cartSerice.GetCart(customerId);

        return View(cart);
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart(int urunId, int miktar = 1)
    {
        await _cartSerice.AddToCart(urunId, miktar);

        return RedirectToAction("Index", "Cart");
        // var item = cart.CartItems.Where(i => i.UrunId == urunId).Any(); // Any() değeri item değerini soruya döndürür ve true false değer döndürür burada girdiğimiz değer varsa true yoksa false döner 
    }

    [HttpPost]
    public async Task<ActionResult> RemoveItem(int urunId, int miktar) {
        await _cartSerice.RemoveItem(urunId, miktar);

        return RedirectToAction("Index", "Cart");
    }
}