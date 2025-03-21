﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Models.Shop;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class ShopController : BaseController
    {
        private readonly IShopService shopService;
        private readonly SignInManager<MyArtPlaceUser> signInManager;

        public ShopController(IShopService _shopService , SignInManager<MyArtPlaceUser> _signInManager)
        {
            shopService = _shopService;
            signInManager = _signInManager;
        }

        public async Task<IActionResult> CreateShop()
        {
            var model = new ShopViewModel();

            await CheckMessages();

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
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccsessfulCreatedShopMessage);
                await signInManager.SignOutAsync();
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Settings()
        {
            try
            {
                await CheckMessages();

                var model = await shopService.GetShopForEdit(User.FindFirstValue(ClaimTypes.NameIdentifier));

                model.AllCurrencies = await shopService.GetAllCurrencies();

                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
                return Redirect("/");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Settings(ShopEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await shopService.EditShop(model ,User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, MessageConstants.SuccessfulSavedChanges);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, MessageConstants.ThereWasErrorMessage);
            }

            return Redirect("/");
        }
    }
}
