namespace dotnet_store.Models;
using System.ComponentModel.DataAnnotations;

public class UrunCreateModel {

    [Display(Name = "Ürün Adı")]
    [Required(ErrorMessage = "Ürün Adı Girmelisiniz.")]
    [StringLength(50, ErrorMessage = "Ürün Adı İçin Maksimum 10-50 Karakter Girmelisiniz.", MinimumLength = 30)]
    public string UrunAdi { get; set; } = null!;
    
    [Display(Name = "Ürün Fiyat")]
    [Required(ErrorMessage = "Urun Fiyat Zorunlu.")]
    [Range(0, 100000, ErrorMessage = "{0} için girdiğiniz değer {1} ile {2} aralığında olmalıdır.")] // Burada bir değeri üstteki Name değerinin içindeki değerdir 1 değeri ve sonrasındaki sayılar bu satırın içindeki değerlerdir
    // {1} değeri 0'a {2} değeri de 100000 değerine karşılık gelir.
    
    public double? Fiyat { get; set; }

    [Display(Name = "Ürün Resmi")]

    public IFormFile Resim { get; set; }

    public string? Aciklama { get; set; }

    public bool Aktif { get; set; }

    public bool Anasayfa { get; set; }

    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "{0} Fiyat Zorunlu.")] // Buraya yazmış olduğum sıfır değeri Display içerisindeki Name değerini alır.
    public int? KategoriId { get; set; }

}

