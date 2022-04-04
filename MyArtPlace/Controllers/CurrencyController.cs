using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Models.Common;

namespace MyArtPlace.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CurrencyController : BaseController
    {
        private readonly ICurrencyService currencyService;

        public CurrencyController(ICurrencyService _currencyService)
        {
            currencyService = _currencyService;
        }

        public async Task<IActionResult> ManageCurrencies()
        {
            await CheckMessages();

            var currencies = await currencyService.AllCurrencies();

            return View(currencies);
        }

        public async Task<IActionResult> Add()
        {
            var category = new AddCurrencyViewModel();

            await CheckMessages();

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
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful added currency!");
            }
            else
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(ManageCurrencies));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await currencyService.DeleteCurrencyById(id))
            {
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful deleted currency!");
            }
            else
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(ManageCurrencies));
        }
    }
}
