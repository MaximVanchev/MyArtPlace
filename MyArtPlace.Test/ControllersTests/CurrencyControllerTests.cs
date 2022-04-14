using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Controllers;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Services;
using MyArtPlace.Infrastructure.Data.Repositories;
using NUnit.Framework;
using MvcContrib.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyArtPlace.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using MyArtPlace.Core.Models.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Models.Product;
using MyArtPlace.Core.Models.Shop;

namespace MyArtPlace.Test.ControllersTests
{
    public class CurrencyControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private CurrencyController currencyController;
        private Currency currency;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICurrencyService , CurrencyService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);

            currencyController.CheckMessages();
        }

        [Test]
        public void WhenGetManageCurrenciesShouldReturnView()
        {
            var result = currencyController.ManageCurrencies().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<IEnumerable<CurrencyListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetAddShouldRetutnView()
        {
            var result = currencyController.Add().Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<AddCurrencyViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenPostAddShouldRedirectToActionAndAddCurrency()
        {
            var model = new AddCurrencyViewModel()
            {
                Iso = "EUR"
            };
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var currencies = repo.All<Currency>();

            var result = currencyController.Add(model).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(currencyController.ManageCurrencies));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulAddedCurrencyMessage);
            Assert.True(currencies.Count() == 2);
        }

        [Test]
        public void WhenPostAddAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var model = new AddCurrencyViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var result = currencyController.Add(model).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(currencyController.ManageCurrencies));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostDeleteAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var model = new AddCurrencyViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var result = currencyController.Delete(Guid.NewGuid()).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(currencyController.ManageCurrencies));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostAddAndModelStateIsNotValidShouldReturnViewAndAddMessage()
        {
            var model = new AddCurrencyViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            currencyController.ModelState.AddModelError("error", "error message");

            var result = currencyController.Add(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenPostDeleteShouldRedirectToActionAndDeleteCurrency()
        {
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var currencies = repo.All<Currency>();

            var result = currencyController.Delete(currency.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(currencyController.ManageCurrencies));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulDeletedCurrencyMessage);
            Assert.True(currencies.Count() == 0);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            currency = new Currency()
            {
                Iso = "USD"
            };

            var currencyService = serviceProvider.GetService<ICurrencyService>();

            currencyController = new CurrencyController(currencyService);

            await repo.AddAsync(currency);
            await repo.SaveChangesAsync();
        }
    }
}
