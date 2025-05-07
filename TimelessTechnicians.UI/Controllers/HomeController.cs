using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TimelessTechnicians.UI.Models;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["ShowSidebar"] = false;
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["ShowSidebar"] = false;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var statusCode = HttpContext.Response.StatusCode;
        var model = new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier,
            Message = statusCode == StatusCodes.Status500InternalServerError ? "An unexpected error occurred." : "An error occurred."
        };

        return View("Error", model);
    }
}
