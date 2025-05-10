using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class AccountResetPasswordModel {

    public string Token { get; set; } = null!;

    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Yeni Parola")]
    [DataType(DataType.Password)] // Bu değer de girilecek data tipinin ne olduğunu belirtiriz.
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "Yeni Parola Tekrar")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parola Eşleşmiyor")] // Burada girmiş olduğum Compare değeri içine girilen değer ile buradaki değeri karşılaştırır.
    public string ConfirmPassword { get; set; } = null!;
    
}
