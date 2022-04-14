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
    public class ShopControllerTests
    {
        private InMemoryDbContext dbContext;
        private FakeSignInManager<MyArtPlaceUser> FakeSignInManager;
        private FakeUserManager<MyArtPlaceUser> FakeUserManager;
        private ServiceProvider serviceProvider;
        private ShopController shopControllerUserOne;
        private ShopController shopControllerUserTwo;
        private ShopController shopControllerUserTree;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;
        private MyArtPlaceUser userTree;

        [SetUp]
        public async Task Setup()
        {
            FakeSignInManager = new FakeSignInManager<MyArtPlaceUser>();

            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);

            shopControllerUserTwo.CheckMessages();
        }

        [Test]
        public void WhenGetCreateShopShouldReturnView()
        {
            var result = shopControllerUserTree.CreateShop().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<ShopViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenPostCreateProductShouldRedirectToIndexAndCreateProduct()
        {
            var model = new ShopViewModel()
            {
                Currency = "USD",
                Description = "dghahsd",
                Name = "ShopUserTree"
            };

            var reuslt = shopControllerUserTree.CreateShop(model).Result as RedirectResult;
            Assert.AreEqual(reuslt.Url, "/");
            Assert.True(userTree.Shop != null);
            Assert.AreEqual(model.Description , userTree.Shop.Description);
            Assert.AreEqual(model.Name , userTree.Shop.Name);
            Assert.AreEqual(model.Currency , userTree.Shop.Currency.Iso);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulCreatedShopMessage);
        }

        [Test]
        public void WhenPostCreateProductAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var model = new ShopViewModel()
            {
                Currency = "USD",
                Description = "dghahsd",
                Name = "ShopUserTree"
            };

            var reuslt = shopControllerUserOne.CreateShop(model).Result as RedirectResult;
            Assert.AreEqual(reuslt.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostCreateProductAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ShopViewModel()
            {
                Currency = "USD",
                Description = "dghahsd",
                Name = "ShopUserTree"
            };

            shopControllerUserOne.ModelState.AddModelError("error", "error message");

            var reuslt = shopControllerUserOne.CreateShop(model).Result as ViewResult;
            Assert.NotNull(reuslt);
            reuslt.Model.Should().BeEquivalentTo(model);
        }

        [Test]
        public void WhenGetSettingsShouldReturnView()
        {
            var result = shopControllerUserTwo.Settings().Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<ShopEditViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetSettingsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var result = shopControllerUserOne.Settings().Result as RedirectResult;
            Assert.AreEqual(result.Url , "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostSettingsShouldRedirectToIndexAndEditShop()
        {
            var model = new ShopEditViewModel()
            {
                Currency = "USD",
                Description = "dhja",
                Name = "NewShop",
                Location = "Bulgaria"
            };

            var result = shopControllerUserTwo.Settings(model).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccessfulSavedChanges);
        }

        [Test]
        public void WhenPostSettingsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var model = new ShopEditViewModel();

            var result = shopControllerUserOne.Settings(model).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostSettingsAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ShopEditViewModel();

            shopControllerUserOne.ModelState.AddModelError("error", "error message");

            var result = shopControllerUserOne.Settings(model).Result as ViewResult;
            Assert.NotNull(result);
            Assert.AreEqual(model , result.Model);
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
                EmailConfirmed = true
            };

            userTwo = new MyArtPlaceUser()
            {
                UserName = "Ivo",
                Email = "max.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "IvoShop",
                    Currency = new Currency()
                    {
                        Iso = "USD"
                    },
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Mishka",
                            Category = new Category()
                            {
                                Name = "Mishki"
                            },
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            userTree = new MyArtPlaceUser()
            {
                UserName = "Pepi",
                Email = "pepi.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true
            };

            var shopService = new ShopService(repo, FakeUserManager);

            var userTwoClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, userTwo.Id)
            }));

            var userOneClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            var userTreeClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTree.UserName),
                new Claim(ClaimTypes.NameIdentifier, userTree.Id)
            }));

            shopControllerUserOne = new ShopController(shopService , FakeSignInManager);
            shopControllerUserTwo = new ShopController(shopService , FakeSignInManager);
            shopControllerUserTree = new ShopController(shopService , FakeSignInManager);

            shopControllerUserTwo.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userTwoClaims }
            };

            shopControllerUserOne.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userOneClaims }
            };

            shopControllerUserTree.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userTreeClaims }
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.AddAsync(userTree);
            await repo.SaveChangesAsync();
        }
    }
}
