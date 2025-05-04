using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountCreateModel {

    [Required]
    [Display(Name = "Ad Soyad")]
    // [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Sadece sayı ve harf giriniz.")]
    public string AdSoyad { get; set; } = null!;

    [Required]
    [Display(Name = "Eposta")]
    [EmailAddress]    
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Parola")]
    [DataType(DataType.Password)] // Bu değer de girilecek data tipinin ne olduğunu belirtiriz.
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "Parola")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parola Eşleşmiyor")] // Burada girmiş olduğum Compare değeri içine girilen değer ile buradaki değeri karşılaştırır.
    public string ConfirmPassword { get; set; } = null!;
    
}
