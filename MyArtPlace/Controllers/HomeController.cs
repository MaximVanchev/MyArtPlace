using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Models;
using System.Diagnostics;

namespace MyArtPlace.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            await CheckMessages();

            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            await CheckMessages();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            await CheckMessages();

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}