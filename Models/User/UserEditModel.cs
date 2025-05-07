using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;

public class UserEditModel  {

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } 

    [Required]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; }

    [Display(Name = "Parola")]
    [DataType(DataType.Password)] 
    public string? Password { get; set; } = null!;

    [Display(Name = "Parola Tekrar")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parola Eşleşmiyor")] 
    public string? ConfirmPassword { get; set; } = null!;

    public IList<string>? SelectedRoles { get; set; }

}
