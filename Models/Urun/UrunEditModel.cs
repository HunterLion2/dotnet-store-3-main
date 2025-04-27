namespace dotnet_store.Models;
using System.ComponentModel.DataAnnotations;

public class UrunEditModel {

    public string UrunAdi { get; set; } = null!;
    
    public double Fiyat { get; set; }

    public string? Resim { get; set; }

    public string? Aciklama { get; set; }

    public bool Aktif { get; set; }

    public bool Anasayfa { get; set; }

    public int KategoriId { get; set; }
    
    public int Id { get; set; }
}

