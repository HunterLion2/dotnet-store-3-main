using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;

public class AccountEditUserModel  {

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } 

    [Required]
    [Display(Name = "Ad Soyad")]
    public string AdSoyad { get; set; }

}
