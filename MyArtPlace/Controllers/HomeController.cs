using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService productService;

        public HomeController(ILogger<HomeController> logger , IProductService _productService)
        {
            _logger = logger;
            productService = _productService;
        }

        public async Task<IActionResult> Index()
        {
            await CheckMessages();

            try
            {
                var model = await productService.GetAllProducts(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
                return Redirect("/");
            }
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