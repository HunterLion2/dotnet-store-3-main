namespace dotnet_store.Models;
using System.ComponentModel.DataAnnotations;

public class KategoriEditModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Kategori Adı")]
    public string KategoriAdi { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [Display(Name = "Url")]
    public string Url { get; set; } = null!;
}

