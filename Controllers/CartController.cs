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

        var urun = await _context.Urunler.FirstOrDefaultAsync(i => i.Id == urunId);

        if(urun != null) {
            cart.AddItem(urun,miktar);
            await _context.SaveChangesAsync();            
        }

        return RedirectToAction("Index", "Cart");

    }

    [HttpPost]
    public async Task<ActionResult> RemoveItem(int urunId, int miktar) {
        var cart = await GetCart();

        var urun = await _context.Urunler.FirstOrDefaultAsync(i => i.Id == urunId);

        if(urun != null) {
            cart.DeleteItem(urunId,miktar);
            await _context.SaveChangesAsync();            
        }

        return RedirectToAction("Index","Cart");
    }

    private async Task<Cart> GetCart()
    {

        // Burada iki soru işaretinin anlamı eğer User.Identity?.Name bölümü null döndürüyorsa o zaman Request.Cookies["customerId"] değerini bana getir yani cookie de olan bölümü bana getir deriz.

        var customerId = User.Identity?.Name ?? Request.Cookies["customerId"];

        var cart = await _context.Carts.Include(i => i.CartItems)
                                       .ThenInclude(i => i.Urun) // Burada yazdığımız ThenInclude değeri , Include ile ulaştığımız diğer birbirine bağlanan değere bağlandıktan sonra o bağlandığımız değerde yine başka bir değer ile bağlı ise ona bağlanmak için yazılır.
                                       .Where(i => i.CustomerId == customerId)
                                       .FirstOrDefaultAsync();

        if (cart == null)
        {
            // Eğer kullanıcı giriş yapmamışsa, ona özel bir müşteri kimliği (customerId) oluşturmak için
            // Guid.NewGuid().ToString() kullanılıyor.

            // Böylece her anonim kullanıcıya benzersiz bir sepet atanabiliyor ve çakışma yaşanmıyor.
            customerId = User.Identity?.Name;

            if(string.IsNullOrEmpty(customerId)) {
                customerId = Guid.NewGuid().ToString(); // Buradaki Guid değeri benzersiz bir değer oluşturur 203492-203942 gibi aslında bu bizim cookie değerimizdir aşşağıdaki bölümde ayarlarıdır bu ayrımı son zaman Append() yaptığımız zaman ayırt ederiz.

                var cookieOptions = new CookieOptions { // Bu değer ile cookie mizi oluştururuz ve aşşağıda da ayarlamaları yaparız.
                    Expires = DateTime.Now.AddMonths(1), // Cookie değerinin ne zaman son bulucağını söyler.
                    IsEssential = true
                };

                Response.Cookies.Append("customerId",customerId,cookieOptions);
            }



            cart = new Cart { CustomerId = customerId };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }
}