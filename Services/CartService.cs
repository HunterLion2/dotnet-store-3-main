using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Services;



public interface ICartService
{
    string GetCustomerId();

    Task<Cart> GetCart(string customerId);

    Task AddToCart(int urunId, int miktar = 1);

    Task RemoveItem(int urunId, int miktar = 1);

    Task TransferCartToUser(string username);
}

public class CartService : ICartService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task AddToCart(int urunId, int miktar = 1)
    {
        var cart = await GetCart(GetCustomerId());

        // var item = cart.CartItems.Where(i => i.UrunId == urunId).Any(); // Any() değeri item değerini soruya döndürür ve true false değer döndürür burada girdiğimiz değer varsa true yoksa false döner 

        var urun = await _context.Urunler.FirstOrDefaultAsync(i => i.Id == urunId);

        if(urun != null) {
            cart.AddItem(urun,miktar);
            await _context.SaveChangesAsync();            
        }
    }

    public async Task<Cart> GetCart(string custId)
    {
    //    var customerId = User.Identity?.Name ?? Request.Cookies["customerId"];

        var cart = await _context.Carts.Include(i => i.CartItems)
                                       .ThenInclude(i => i.Urun) // Burada yazdığımız ThenInclude değeri , Include ile ulaştığımız diğer birbirine bağlanan değere bağlandıktan sonra o bağlandığımız değerde yine başka bir değer ile bağlı ise ona bağlanmak için yazılır.
                                       .Where(i => i.CustomerId == custId)
                                       .FirstOrDefaultAsync();

        if (cart == null)
        {
            // Eğer kullanıcı giriş yapmamışsa, ona özel bir müşteri kimliği (customerId) oluşturmak için
            // Guid.NewGuid().ToString() kullanılıyor.

            // Böylece her anonim kullanıcıya benzersiz bir sepet atanabiliyor ve çakışma yaşanmıyor.
            var customerId = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            if(string.IsNullOrEmpty(customerId)) {
                customerId = Guid.NewGuid().ToString(); // Buradaki Guid değeri benzersiz bir değer oluşturur 203492-203942 gibi aslında bu bizim cookie değerimizdir aşşağıdaki bölümde ayarlarıdır bu ayrımı son zaman Append() yaptığımız zaman ayırt ederiz.

                var cookieOptions = new CookieOptions { // Bu değer ile cookie mizi oluştururuz ve aşşağıda da ayarlamaları yaparız.
                    Expires = DateTime.Now.AddMonths(1), // Cookie değerinin ne zaman son bulucağını söyler.
                    IsEssential = true
                };

                _httpContextAccessor.HttpContext?.Response.Cookies.Append("customerId",customerId,cookieOptions);
            }



            cart = new Cart { CustomerId = customerId };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    public string GetCustomerId()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.User.Identity?.Name ?? context?.Request.Cookies["customerId"]!;
    }

    public async Task RemoveItem(int urunId, int miktar = 1)
    {
        var cart = await GetCart(GetCustomerId());

        var urun = await _context.Urunler.FirstOrDefaultAsync(i => i.Id == urunId);

        if(urun != null) {
            cart.DeleteItem(urunId,miktar);
            await _context.SaveChangesAsync();            
        }
    }

    public async Task TransferCartToUser(string username)
    {
        var userCart = await GetCart(username);

        var cookieCart = await GetCart(_httpContextAccessor.HttpContext?.Request.Cookies["customerId"]!);

        foreach (var item in cookieCart?.CartItems!)
        {

            var cartItem = userCart?.CartItems.Where(i => i.UrunId == item.UrunId).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Miktar += item.Miktar;
            }
            else
            {
                userCart?.CartItems.Add(new CartItem
                {
                    UrunId = item.UrunId,
                    Miktar = item.Miktar
                });
            }

            _context.Carts.Remove(cookieCart);

            await _context.SaveChangesAsync();
        }
    }
}

