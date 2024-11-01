using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class SpaceController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}