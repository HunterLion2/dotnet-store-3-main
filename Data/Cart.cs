namespace dotnet_store.Models;

public class Cart {
    public int CartId { get; set; }
    public string CustomerId { get; set; } = null!;
    public List<CartItem> CartItems { get; set; } = new();
}

public class CartItem {
    public int CartItemId { get; set; }
    public int UrunId { get; set; }
    public Urun Urun { get; set; } = null!; // Buralarda bir tablodan bu tabloya bilgi çekiyoruz

    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;
    public int Miktar { get; set; }
}
