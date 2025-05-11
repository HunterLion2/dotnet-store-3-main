using System.Threading.Tasks;
using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;


[Authorize] // Ben bu değer ile admincontroller değerini sadece Roles değeri Admin değerine sahip olanlar girebilir derim
public class CartController : Controller
{

    private readonly DataContext _context;

    public CartController(DataContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart(int urunId, int miktar = 1)
    {
        var customerId = User.Identity?.Name;

        var cart = await _context.Carts.Include(i => i.CartItems).Where(i => i.CustomerId == customerId).FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new Cart { CustomerId = customerId! };
            _context.Carts.Add(cart);
        }

        // var item = cart.CartItems.Where(i => i.UrunId == urunId).Any(); // Any() değeri item değerini soruya döndürür ve true false değer döndürür burada girdiğimiz değer varsa true yoksa false döner 
        var item = cart.CartItems.Where(i => i.UrunId == urunId).FirstOrDefault();

        if (item != null)
        {
            item.Miktar += 1;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                UrunId = urunId,
                Miktar = miktar
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index","Home");

    }
}