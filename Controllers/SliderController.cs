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



}
