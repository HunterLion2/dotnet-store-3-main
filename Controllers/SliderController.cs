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



}
