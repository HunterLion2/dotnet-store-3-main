using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace dotnet_store.Controllers;

[Authorize(Roles = "Admin")]
public class UrunController : Controller
{
    private readonly DataContext _context;

    
    public UrunController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index(int? kategori)
    {
        // AsQueryable() değeri ile oluşturulan sorgular yalnızca sonuçlara erişildiğinde (örneğin, ToList() çağrıldığında) çalıştırılır.
        var query = _context.Urunler.AsQueryable();

        if(kategori != null) {
            query = query.Where(i => i.KategoriId == kategori);
        }

        var urunler = query.Select(i => new UrunGetModel
        {
            Id = i.Id,
            UrunAdi = i.UrunAdi,
            Fiyat = i.Fiyat,
            Aktif = i.Aktif,
            Anasayfa = i.Anasayfa,
            KategoriAdi = i.Kategori.KategoriAdi,
            Resim = i.Resim
        }).ToList();
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi", kategori);

        return View(urunler);
    }

    [AllowAnonymous] // Bunu yazdığım zaman dediğim zaman Roles değeri yani yukarıda belirlemiş olduğum değere sen girmiyorsun yani burayı görmeleri için Roles değeri Admin olmasına gerek yoktur derim.
    public ActionResult List(string url, string q)
    {
        var query = _context.Urunler.Where(i => i.Aktif);

        if (!string.IsNullOrEmpty(url))
        {
            query = query.Where(i => i.Kategori.Url == url);
        }

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(i => i.UrunAdi.ToLower().Contains(q.ToLower()));

            ViewData["q"] = q;
        }

        return View(query.ToList());
    }

     [AllowAnonymous]
    public ActionResult Details(int id)
    {
        var urun = _context.Urunler.Find(id);

        if (urun == null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["BenzerUrunler"] = _context.Urunler
                                        .Where(i => i.Aktif && i.KategoriId == urun.KategoriId && i.Id != id)
                                        .Take(4)
                                        .ToList();

        return View(urun);
    }

    public ActionResult Create()
    {
        // Buradaki Gibi de Olur
        // ViewData["Kategoriler"] = _context.Kategoriler.toList();

        // Buradaki Gibi De Olur
        // ViewBag.Kategoriler = _context.Kategoriler.ToList();

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View();
    }

    [HttpPost]
    // Aşşağıda ki Bind kısmı model içerisinde ki bilgilerden seçmece yapıp istediğimizi getirmemizi sağlar.
    // public ActionResult Create([Bind("UrunAdi", "Aciklama")]UrunCreateModel model)
    public async Task<ActionResult> CreateAsync(UrunCreateModel model)
    {
        if (model.Resim == null || model.Resim.Length == 0)
        {
            ModelState.AddModelError("Resim", "Resim Seçmelisiniz"); // İlk girilen değer hatanın hangi değer ile eşleşeceğidir , ikinci değer ise hata sonucu çıkıcak string değerdir yani hata mesajıdır.
        }

        if (ModelState.IsValid)
        {
            // Buradaki Path.GetRandomFileName() methodu sayesinde random bir dosya ismi oluşturmuş oluruz bu sayede aynı dosya ismi kazara girilmemiş olur.
            var fileName = Path.GetRandomFileName() + "jpg";

            // Burada yapmış olduğum işlem mevcut olan ana dizin alıp bu yola  wwwroot/img bunu eklemiş olurum.
            // En sona da içerine atacağım değeri yazarım.
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            // FileStream: Dosya üzerinde okuma/yazma işlemleri yapmak için kullanılan bir sınıftır.
            // path: Dosyanın kaydedileceği tam dosya yolu. Örneğin: "wwwroot/img/example.jpg".
            // FileMode.Create: Eğer belirtilen dosya zaten varsa, üzerine yazar. Eğer dosya yoksa, yeni bir dosya oluşturur.
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await model.Resim!.CopyToAsync(stream);
            }

            var entity = new Urun()
            {
                UrunAdi = model.UrunAdi,
                Aciklama = model.Aciklama,
                Fiyat = model.Fiyat ?? 0,
                Aktif = model.Aktif,
                Anasayfa = model.Anasayfa,
                KategoriId = (int)model.KategoriId!,
                Resim = fileName
            };

            _context.Urunler.Add(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(model);
    }

    public ActionResult Edit(int id)
    {

        var entity = _context.Urunler.Select(i => new UrunEditModel
        {
            UrunAdi = i.UrunAdi,
            Aciklama = i.Aciklama,
            Fiyat = i.Fiyat,
            Aktif = i.Aktif,
            ResimAdi = i.Resim,
            Anasayfa = i.Anasayfa,
            Id = i.Id,
            KategoriId = i.KategoriId

        }).FirstOrDefault(i => i.Id == id);

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(entity);
    }

    [HttpPost]
    public async Task<ActionResult> EditAsync(int id, UrunEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            var entity = _context.Urunler.FirstOrDefault(i => i.Id == model.Id);

            if (entity != null)
            {

                if (model.Resim != null)
                {
                    var fileName = Path.GetRandomFileName() + "jpg"; // Random bir dosya ismi tanımla
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName); // Bir konum belirle

                    using (var stream = new FileStream(path, FileMode.Create))
                    {// Burada da ilgili konuma istediğimiz değeri atarız.
                        await model.Resim!.CopyToAsync(stream);
                    }

                    entity.Resim = fileName;
                }

                entity.UrunAdi = model.UrunAdi;
                entity.Aciklama = model.Aciklama;
                entity.Aktif = model.Aktif;
                entity.Anasayfa = model.Anasayfa;
                // ?? Değeri yazdığımız elementin null olup olmadığına bakar eğer null ise 0 değerini verir.
                entity.Fiyat = model.Fiyat ?? 0;
                entity.KategoriId = (int)model.KategoriId!; // (int) değerine çeviririz ve sonrasında da ! diyerek bu değer kesinlikle boş kalmıyacak deriz

                _context.SaveChanges();

                TempData["Message"] = $"{entity.UrunAdi} Kategorisi Güncellendi";

                return RedirectToAction("Index");
            }
        }
        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");
        return View(model);
    }

    public ActionResult Delete(int? id) {

        if(id == null) {
            return RedirectToAction("Index");
        }

        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);

        if(entity != null) {
            return View(entity);  
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteConfirm(int? id) {

        if(id == null) {
            return RedirectToAction("Index");
        }

        var entity = _context.Urunler.FirstOrDefault(i => i.Id == id);

        if(entity != null) {
            _context.Urunler.Remove(entity);
            _context.SaveChanges();

            TempData["Message"] = $"{entity.UrunAdi} Ürünü Silindi";
        }
        return RedirectToAction("Index");
    }

}