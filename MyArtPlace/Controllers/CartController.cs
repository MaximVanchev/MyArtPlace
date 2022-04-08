using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Core.Models.Common;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService cartService;

        public CartController(ICartService _cartService)
        {
            cartService = _cartService;
        }

        public async Task<IActionResult> UserCart()
        {
            await CheckMessages();

            try
            {
                var model = await cartService.GetUserCart(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> UserCart(CartListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                return RedirectToAction(nameof(CartSubmit), new { iso = model.Currency });
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return RedirectToAction(nameof(UserCart));
        }

        public async Task<IActionResult> CartSubmit(string iso)
        {
            await CheckMessages();

            try
            {
                var model = await cartService.GetSubmitModel(User.FindFirstValue(ClaimTypes.NameIdentifier) , iso);
                return View(model);
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> CartSubmit(CartAddressSubmitViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await cartService.SubmitOrder(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulSubmittedOrderMessage);
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception ex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCart(Guid productId)
        {
            try
            {
                await cartService.AddProductToCart(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulAddedProductToCartMessage);
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProductFromCart(Guid cartId)
        {
            try
            {
                await cartService.RemoveProductFromCart(cartId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulRemovedProductFromCartMessage);
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return RedirectToAction(nameof(UserCart));
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseProductCount(Guid cartId)
        {
            try
            {
                await cartService.IncreaseProductCount(cartId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return RedirectToAction(nameof(UserCart));
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseProductCount(Guid cartId)
        {
            try
            {
                await cartService.DecreaseProductCount(cartId, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return RedirectToAction(nameof(UserCart));
        }
    }
}
