using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class SliderController: Controller {

    private readonly DataContext _context;

    public SliderController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index() {

        var slider = _context.Sliderlar.ToList();
        return View(slider);
    }

    public ActionResult Create() {
        return View();
    }

    [HttpPost]
    public ActionResult Create(SliderGetModel model) {

        var entity = new Slider {
            Baslik = model.Baslik,
            Aciklama = model.Aciklama,
            Resim = "slider-1.jpeg",
            Index = model.Index,
            Aktif = model.Aktif
        };

        _context.Sliderlar.Add(entity);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public ActionResult Edit(int id) {
        var slider = _context.Sliderlar.Select(i => new SliderEditModel {
            Id = i.Id,
            Baslik = i.Baslik,
            Aciklama = i.Aciklama,
            Resim = i.Resim,
            Index = i.Index,
            Aktif = i.Aktif
        }).FirstOrDefault(i => i.Id == id);
        
        return View(slider);
    }

    [HttpPost]
    public async Task<ActionResult> EditAsync(int id, SliderEditModel model) {

        if(model.Id != id) {
            return RedirectToAction("Index");
        }

        var entity = _context.Sliderlar.FirstOrDefault(i => i.Id == model.Id);

        if(entity != null) {

            if(model.ResimDosyası != null) {
                var fileName = Path.GetRandomFileName() + "jpg";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img",fileName);

                using(var stream = new FileStream(path, FileMode.Create)) {// Burada da ilgili konuma istediğimiz değeri atarız.
                    await model.ResimDosyası!.CopyToAsync(stream);
                }

            }
            
            entity.Id = model.Id;
            entity.Aciklama = model.Aciklama;
            entity.Aktif = model.Aktif;
            entity.Baslik = model.Baslik;
            entity.Index = model.Index;
            entity.Resim = model.Resim;

            _context.SaveChanges();

            TempData["Message"] = $"{entity.Baslik} Kategorisi Güncellendi";

            return RedirectToAction("Index");
        }


        return View(model);
    }



}
