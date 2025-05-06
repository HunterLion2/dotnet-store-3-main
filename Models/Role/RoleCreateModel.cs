namespace dotnet_store.Models;
using System.ComponentModel.DataAnnotations;
public class RoleCreateModel
{
    // Burada köşeli parantez içerisinde girdiğim değerler validation değerleridir.

    // [Required] => Bu değer aşşağıda ki KategoriAdi değerinin zorunlu olarak girilmesi gerektiğini vurgular , girilmezse hata verdirir.
    // [StringLenght(20)] => Bu değer KategoriAdi değerinin içerisine girilecek değerin maksimum uzunluğunu içeri girdiğimiz bir değer ile belirleriz.

    [Required]
    [StringLength(20)]
    [Display(Name = "Rol Adı")]
    public string RoleAdi { get; set; } = null!;

}

