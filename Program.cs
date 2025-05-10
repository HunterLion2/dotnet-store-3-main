using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IEmailService, SmtpEmailService>(); // Burada demek istediğimiz şey IEmailService interface'i çağırdığımız zaman bana SmtpEmailService servisini çağır yani onu kullanıcam anlamına gelir.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

// Burada yazmış olduğumuz  AddIdentity ile İdentity değeri sayesinde IdentityUser ve IdentityRole adlı tabloları kullanabilir hale geliriz.
// AddEntityFrameworkStores bu özellik de oluşturulacak tabloların nerede oluşturulacağını söylemiş oluruz.

// builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataContext>();

// Burada artık IdentityUser yerine kendi oluşturmuş olduğumuz AppUser'ı kullanıyoruz aşşağıdaki Role içinde bu geçerli.
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders(); // AddDefaultTokenProviders özelliği ile uygulamaya Token oluşturma yeteneği kazandırırız.

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequiredLength = 7;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;

    options.User.RequireUniqueEmail = true;
    // options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789@"; 

    options.Lockout.MaxFailedAccessAttempts = 5; // Burada yazdığım sayı değeri kişi hesabına eğer 5 den fazla hatalı giriş yaparsak lockout yap onu der.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Burada da Lockout işleminin ne kadar süreceğini söyleriz ben burada 5 dakika demiş oldum.
});

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Kişinin siteye giriş yaptıktan ne kadar süre sonra oturumu kapatılsın ve tekrar şifre mail istensin demektir burada yazdığımız
    options.SlidingExpiration = true; // Burada ki değer de true olduğu zaman site ile etkileşim olduğu zamanlar yukarıdaki TimeSpan kendini sıfırlar ve tekrar kendini 30 güne alır burayı false yaparsak etkileşin olsa bile 30 gün sonra oturum kapatılır.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// urunler/telefon
// urunler/elektronik
// urunler/beyaz-esya

app.MapControllerRoute(
    name: "urunler_by_kategori",
    pattern: "urunler/{url?}",
    defaults: new { controller = "Urun", action = "List" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
