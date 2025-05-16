using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class Order
{
    public int SiparisId { get; set; }

    public DateTime SiparisTarihi { get; set; }

    public string UserName { get; set; } = null!;

    public string Sehir { get; set; } = null!;

    public string AdresSatiri { get; set; } = null!;

    public string PostaKodu { get; set; } = null!;

    public string Telefon { get; set; } = null!;

    public string Email { get; set; } = null!;

    public double ToplamFiyat { get; set; }
}

public class OrderItem
{
    public int OrderItemId { get; set; }
}

