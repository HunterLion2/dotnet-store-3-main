using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;

public class AppUser : IdentityUser<int> {

    public string AsSoyad { get; set; } = null!;

}
