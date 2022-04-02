using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;

namespace MyArtPlace.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService currencyService;

        public CurrencyController(ICurrencyService _currencyService)
        {
            currencyService = _currencyService;
        }

        public async Task<IActionResult> ManageCurrencies()
        {
            var currencies = await currencyService.AllCurrencies();

            return View(currencies);
        }

        public async Task<IActionResult> Add()
        {
            var category = new AddCurrencyViewModel();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCurrencyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await currencyService.AddCurrency(model))
            {
                ViewData[MessageConstants.SuccessMessage] = "Successful added category!";
            }
            else
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return RedirectToAction(nameof(ManageCurrencies));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await currencyService.DeleteCurrencyById(id))
            {
                ViewData[MessageConstants.SuccessMessage] = "Succsessful deleted currency!";
            }
            else
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return RedirectToAction(nameof(ManageCurrencies));
        }
    }
}
