using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Core.Services;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MyArtPlace.Core.Models.User;
using Microsoft.AspNetCore.Http;
using System.IO;
using MyArtPlace.Core.Models.Shop;

namespace MyArtPlace.Test.ServicesTests
{
    public class ShopServiceTests
    {
        FakeUserManager<MyArtPlaceUser> userManager;
        private ShopService shopService;
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;
        private Currency currency;

        [SetUp]
        public async Task SetUp()
        {
            userManager = new FakeUserManager<MyArtPlaceUser>();
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            shopService = new ShopService(repo, userManager);

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenCreateShopShouldBeCreated()
        {
            var model = new ShopViewModel()
            {
                Name = "PeshoShop",
                Currency = "USD",
                Description = "shop for toys",
                Location = "Geo Milev Bulgaria"
            };

            shopService.CreateShop(model, userOne.Id);

            Assert.True(userOne.Shop != null);
        }

        [Test]
        public void WhenGetAllCurrenciesShouldReturnThemCorrect()
        {
            var allCurrencies = new List<Currency>();

            allCurrencies.Add(userOne.Shop.Currency);
            allCurrencies.Add(currency);

            allCurrencies.Should().BeEquivalentTo(shopService.GetAllCurrencies().Result.ToList());
        }

        [Test]
        public void WhenGetCurrencyByIsoShouldReturnCurrency()
        {
            var currency = userOne.Shop.Currency;

            currency.Should().BeEquivalentTo(shopService.GetCurrencyByIso(currency.Iso).Result);
        }

        [Test]
        public void WhenUserGetShopForEditShouldReturnShop()
        {
            var model = new ShopEditViewModel()
            {
                Name = userOne.Shop.Name,
                Currency = userOne.Shop.Currency.Iso,
                Location = userOne.Shop.Location,
                Description = userOne.Shop.Description
            };

            model.Should().BeEquivalentTo(shopService.GetShopForEdit(userOne.Id).Result);
        }

        [Test]
        public void WhenUserEditShopShouldBeEdited()
        {
            var model = new ShopEditViewModel()
            {
                Name = "ToyShop",
                Currency = "EUR",
                Location = "France",
                Description = "shop"
            };

            shopService.EditShop(model, userOne.Id);

            Assert.AreEqual(model.Name, userOne.Shop.Name);
            Assert.AreEqual(model.Currency, userOne.Shop.Currency.Iso);
            Assert.AreEqual(model.Description, userOne.Shop.Description);
            Assert.AreEqual(model.Location, userOne.Shop.Location);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            userOne = new MyArtPlaceUser()
            {
                UserName = "Maxi",
                Email = "maxim.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "MaxiShop",
                    Currency = new Currency()
                    {
                        Iso = "BGN"
                    },
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Kotka",
                            Category = new Category()
                            {
                                Name = "Prints"
                            },
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            userTwo = new MyArtPlaceUser()
            {
                UserName = "Pesho",
                Email = "maxi.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true
            };

            currency = new Currency()
            {
                Iso = "EUR"
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.AddAsync(currency);
            await repo.SaveChangesAsync();
        }
    }
}
