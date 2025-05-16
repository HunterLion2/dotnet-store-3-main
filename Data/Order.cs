using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class Order
{
    public int Id { get; set; }

    public DateTime SiparisTarihi { get; set; }

    public string UserName { get; set; } = null!;

    public string Sehir { get; set; } = null!;

    public string AdresSatiri { get; set; } = null!;

    public string PostaKodu { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public string Email { get; set; } = null!;

    public double ToplamFiyat { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public int UrunId { get; set; }

    public Urun Urun { get; set; } = null!;

    public double Fiyat { get; set; }

    public int Miktar { get; set; }
}

