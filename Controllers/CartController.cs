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

    public async Task<ActionResult> Index()
    {
        var cart = await GetCart();

        return View(cart);
    }

    [HttpPost]
    public async Task<ActionResult> AddToCart(int urunId, int miktar = 1)
    {
        var cart = await GetCart();

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

        return RedirectToAction("Index", "Cart");

    }

    private async Task<Cart> GetCart()
    {

        var customerId = User.Identity?.Name;

        var cart = await _context.Carts.Include(i => i.CartItems)
                                       .ThenInclude(i => i.Urun) // Burada yazdığımız ThenInclude değeri , Include ile ulaştığımız diğer birbirine bağlanan değere bağlandıktan sonra o bağlandığımız değerde yine başka bir değer ile bağlı ise ona bağlanmak için yazılır.
                                       .Where(i => i.CustomerId == customerId)
                                       .FirstOrDefaultAsync();

        if (cart == null)
        {
            cart = new Cart { CustomerId = customerId! };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }
}