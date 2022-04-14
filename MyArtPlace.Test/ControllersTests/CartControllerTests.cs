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

namespace MyArtPlace.Test.ControllersTests
{
    public class CartControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private CartController cartControllerUserOne;
        private CartController cartControllerUserTwo;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICartService, CartService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenGetUserCartAndThereIsErrorShouldRedirectToIndex()
        {
            var result = cartControllerUserOne.UserCart().Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
        }

        [Test]
        public void WhenGetUserCartShouldReturnView()
        {
            var result = cartControllerUserTwo.UserCart().Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<CartListViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenPostUserCartShouldRedirectToAction()
        {
            var model = new CartListViewModel()
            {
                Currency = "BGN"
            };

            var result = cartControllerUserOne.UserCart(model).Result as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.AreEqual(model.Currency, result.RouteValues.First().Value);
            Assert.AreEqual(result.ActionName, nameof(cartControllerUserOne.CartSubmit));
        }

        [Test]
        public void WhenPostUserCartWithNotValidStateShouldReturnView()
        {
            var model = new CartListViewModel()
            {
                Currency = "BGN"
            };

            cartControllerUserTwo.ModelState.AddModelError("error", "error message");

            var result = cartControllerUserTwo.UserCart(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenGetCartSubmitShouldReturnView()
        {
            string iso = "BGN";

            var result = cartControllerUserTwo.CartSubmit(iso).Result as ViewResult;
            Assert.NotNull(result);
        }

        [Test]
        public void WhenGetCartSubmitAndThereIsArgumentExShouldRedirectToIndexAndAddMessage()
        {
            string iso = "BGN";

            var cart = userTwo.CartProducts.First();

            cartControllerUserTwo.RemoveProductFromCart(cart.OrderId);

            cartControllerUserTwo.CheckMessages();

            var result = cartControllerUserTwo.CartSubmit(iso).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.NoProductsInTheCartSubmitErrorMessage);
        }

        [Test]
        public void WhenPostCartSubmitShouldRedirectToIndexAndSubmitOrder()
        {
            var model = new CartAddressSubmitViewModel()
            {
                Currency = "BGN",
                OrederAddress = "Bulgaria",
                TotalPrice = 100
            };

            var result = cartControllerUserTwo.CartSubmit(model).Result as RedirectResult;
            Assert.True(userTwo.CartProducts.First().InCart == false);
            Assert.AreEqual(result.Url, "/");
        }

        [Test]
        public void WhenPostCartSubmitWithNotValidStateShouldReturnView()
        {
            var model = new CartAddressSubmitViewModel()
            {
                Currency = "BGN",
                OrederAddress = "Bulgaria",
                TotalPrice = 100
            };

            cartControllerUserTwo.ModelState.AddModelError("error", "error message");

            var result = cartControllerUserTwo.CartSubmit(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenPostIncreaseProductCountShouldRedirectToAction()
        {
            var product = userOne.Shop.Products.First();

            var result = cartControllerUserTwo.IncreaseProductCount(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(cartControllerUserOne.UserCart));
        }

        [Test]
        public void WhenPostDecreaseProductCountShouldRedirectToAction()
        {
            var product = userOne.Shop.Products.First();

            var result = cartControllerUserTwo.DecreaseProductCount(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(cartControllerUserOne.UserCart));
        }

        [Test]
        public void WhenPostAddProductToCartShouldRedirectToIndexAndAddProduct()
        {
            var product = userOne.Shop.Products.Last();

            var result = cartControllerUserTwo.AddProductToCart(product.Id).Result as RedirectResult;
            Assert.True(userTwo.CartProducts.Count() == 2);
            Assert.AreEqual(result.Url, "/");
        }

        [Test]
        public void WhenPostAddProductToCartAndThereIsArgumentExShouldAddArgumentExMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = cartControllerUserTwo.AddProductToCart(product.Id).Result as RedirectResult;
            Assert.True(MessageViewModel.Message[MessageConstants.ErrorMessage] == MessageConstants.UserOwnProductAddToCartErrorMessage);
            Assert.AreEqual(result.Url, "/");
        }

        [Test]
        public void WhenPostRemoveProductFromCartShouldRedirectToActionAndRemoveProduct()
        {
            var cart = userTwo.CartProducts.First();

            var result = cartControllerUserTwo.RemoveProductFromCart(cart.OrderId).Result as RedirectToActionResult;
            Assert.True(userTwo.CartProducts.Count() == 0);
            Assert.AreEqual(result.ActionName, nameof(cartControllerUserOne.UserCart));
        }

        [Test]
        public void WhenPostRemoveProductFromCartAndThereIsErrorShouldAddErrorMessage()
        {
            var product = userOne.Shop.Products.Last();

            var result = cartControllerUserOne.RemoveProductFromCart(product.Id).Result as RedirectToActionResult;
            Assert.True(MessageViewModel.Message[MessageConstants.ErrorMessage] != null);
            Assert.AreEqual(result.ActionName, nameof(cartControllerUserOne.UserCart));
        }

        [Test]
        public void WhenGetCartSubmitAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            string iso = "BGN";

            var result = cartControllerUserOne.CartSubmit(iso).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostCartSubmitAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var model = new CartAddressSubmitViewModel();

            var result = cartControllerUserOne.CartSubmit(model).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostAddProductToCartAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var product = userOne.Shop.Products.First();

            var result = cartControllerUserOne.AddProductToCart(product.Id).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            cartControllerUserTwo.CheckMessages();

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
                        },
                        new Product()
                        {
                            Name = "Kon",
                            Category = new Category()
                            {
                                Name = "Kone"
                            },
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            userTwo = new MyArtPlaceUser()
            {
                UserName = "Ivo",
                Email = "max.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                CartProducts = new List<Cart>()
                {
                    new Cart()
                    {
                        Product = userOne.Shop.Products.First(x => x.Name == "Kotka")
                    }
                },
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

            var cartService = serviceProvider.GetService<ICartService>();

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, userTwo.Id)
            }));

            var userErrorClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            cartControllerUserOne = new CartController(cartService);
            cartControllerUserTwo = new CartController(cartService);

            cartControllerUserOne.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userErrorClaims }
            };

            cartControllerUserTwo.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.SaveChangesAsync();
        }
    }
}
