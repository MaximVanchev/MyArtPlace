using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Shop;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IShopService shopService;

        public ShopController(IShopService _shopService)
        {
            shopService = _shopService;
        }

        public async Task<IActionResult> CreateShop()
        {
            var model = new ShopViewModel();

            model.AllCurrencies = await shopService.GetAllCurrencies();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop(ShopViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await shopService.CreateShop(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData[MessageConstants.SuccessMessage] = "Succsessful created Shop!";
            }
            catch (Exception)
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return Redirect("/");
        }
    }
}
