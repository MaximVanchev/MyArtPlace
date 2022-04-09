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

namespace MyArtPlace.Test.ServicesTests
{
    public class CartServiceTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;
        private MyArtPlaceUser userTree;

        [SetUp]
        public async Task SetUp()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICartService , CartService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenProductIsAddedAgainToCartMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            service.AddProductToCart(userTwo.Shop.Products.First().Id, userOne.Id);

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.AddProductToCart(userTwo.Shop.Products.First().Id, userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.AddedProductAgainToCartErrorMessage);
        }

        [Test]
        public void WhenUserAddOwnProductToCartMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.AddProductToCart(userTwo.Shop.Products.First().Id, userTwo.Id));
            Assert.AreEqual(ex.Message, MessageConstants.UserOwnProductAddToCartErrorMessage);
        }

        [Test]
        public void WhenProductIsOrderedAndAddedAgainMustNotThrow()
        {
            var service = serviceProvider.GetService<ICartService>();
            var model = new CartAddressSubmitViewModel()
            {
                Currency = "USD",
                OrederAddress = "Geo Milev Bulgaria",
                TotalPrice = 100
            };

            service.AddProductToCart(userTwo.Shop.Products.First().Id, userOne.Id);
            service.SubmitOrder(model, userOne.Id);

            Assert.DoesNotThrowAsync(async () =>
            await service.AddProductToCart(userTwo.Shop.Products.First().Id, userOne.Id));
        }

        [Test]
        public void WhenProductIsAddedToCartAndUserDontHaveShopMustNotThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            Assert.DoesNotThrowAsync(async () =>
            await service.AddProductToCart(userTwo.Shop.Products.First().Id, userTree.Id));
        }

        [Test]
        public void WhenUserAddProductToCartShoudBeAdded()
        {
            var service = serviceProvider.GetService<ICartService>();

            service.AddProductToCart(userTwo.Shop.Products.First().Id, userOne.Id);

            Assert.True(userOne.CartProducts.Count() == 1);
        }

        [Test]
        public void WhenUserRemoveProductToCartShoudBeRemoved()
        {
            var service = serviceProvider.GetService<ICartService>();
            Guid productId = userTwo.Shop.Products.First().Id;

            service.AddProductToCart(productId, userOne.Id);
            Guid cardId = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId).OrderId;
            service.RemoveProductFromCart(cardId , userOne.Id);

            Assert.True(userOne.CartProducts.Count() == 0);
        }

        [Test]
        public void WhenProductIsNotInTheCartAndTryToRemoveMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.RemoveProductFromCart(Guid.NewGuid() , userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.NoProductToRemoveErrorMessage);
        }

        [Test]
        public void WhenProductIsNotInTheCartAndTryToIncreaseCountMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.IncreaseProductCount(Guid.NewGuid(), userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.NoProductToIncreaseErrorMessage);
        }

        [Test]
        public void WhenUserTryToIncreaseMoreThanTenProductCountMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            Guid productId = userTwo.Shop.Products.First().Id;

            service.AddProductToCart(productId, userOne.Id);
            Guid cardId = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId).OrderId;
            userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId).ProductConut = 10;
            repo.SaveChangesAsync();

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.IncreaseProductCount(cardId, userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.CantBuyMoreThanTenErrorMessage);
        }

        [Test]
        public void WhenUserIncreaseProductCountShouldBeIncreased()
        {
            var service = serviceProvider.GetService<ICartService>();
            Guid productId = userTwo.Shop.Products.First().Id;

            service.AddProductToCart(productId, userOne.Id);
            var card = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId);
            service.IncreaseProductCount(card.OrderId, userOne.Id);

            Assert.AreEqual(card.ProductConut, 2);
        }

        [Test]
        public void WhenProductIsNotInTheCartAndTryToDecreaseCountMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.DecreaseProductCount(Guid.NewGuid(), userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.NoProductToDecreaseErrorMessage);
        }

        [Test]
        public void WhenUserTryToDecreaseLessThanOneProductCountMustThrow()
        {
            var service = serviceProvider.GetService<ICartService>();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            Guid productId = userTwo.Shop.Products.First().Id;

            service.AddProductToCart(productId, userOne.Id);
            Guid cardId = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId).OrderId;

            var ex = Assert.CatchAsync<ArgumentException>(async () =>
            await service.DecreaseProductCount(cardId, userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.CantBuyLessThanOneErrorMessage);
        }

        [Test]
        public void WhenUserDecreaseProductCountShouldBeDecreased()
        {
            var service = serviceProvider.GetService<ICartService>();
            Guid productId = userTwo.Shop.Products.First().Id;

            service.AddProductToCart(productId, userOne.Id);
            var card = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId);
            service.IncreaseProductCount(card.OrderId, userOne.Id);
            service.DecreaseProductCount(card.OrderId, userOne.Id);

            Assert.AreEqual(card.ProductConut, 1);
        }

        [Test]
        public void WhenUserSubmitCartShouldRemoveProductsFromCart()
        {
            var service = serviceProvider.GetService<ICartService>();
            Guid productId = userTwo.Shop.Products.First().Id;
            var model = new CartAddressSubmitViewModel()
            {
                Currency = "BGN",
                OrederAddress = "Geo Milev Bulgaria",
                TotalPrice = 100
            };

            service.AddProductToCart(productId, userOne.Id);
            service.SubmitOrder(model, userOne.Id);
            var card = userOne.CartProducts.FirstOrDefault(c => c.ProductId == productId);

            Assert.False(card.InCart);
        }

        [Test]
        public void WhenUserGetUserCartShouldReternUserCart()
        {
            var service = serviceProvider.GetService<ICartService>();
            Guid productId = userTwo.Shop.Products.First().Id;
            service.AddProductToCart(productId, userOne.Id);
            var model = new CartListViewModel()
            {
                CartProducts = userOne.CartProducts.Select(p => new CartViewModel()
                {
                    CartId = p.OrderId,
                    ProductId = p.ProductId,
                    ProductName = p.Product.Name,
                    ProductPrice = p.Product.Price,
                    ProductIso = p.Product.Shop.Currency.Iso,
                    ProductCount = p.ProductConut
                }).ToList(),
                AllCurrencies = service.GetAllCurrencies().Result
            };

            var result = service.GetUserCart(userOne.Id).Result;

            model.Should().BeEquivalentTo(result);
        }

        [Test]
        public void WhenGetSubmitModelShouldReurnModel()
        {
            var service = serviceProvider.GetService<ICartService>();
            var currency = "BNG";
            var productOneId= userOne.Shop.Products.First().Id;
            var productOnePrice = userOne.Shop.Products.First().Price;
            var productTwoId = userTwo.Shop.Products.First().Id;
            var productTwoPrice = userTwo.Shop.Products.First().Price;
            var model = new CartAddressSubmitViewModel()
            {
                Currency = currency,
                TotalPrice = service.GetTotalPrice(0, productOnePrice, productTwoPrice, currency).Result
            };

            service.AddProductToCart(productOneId, userTree.Id);
            service.AddProductToCart(productTwoId, userTree.Id);

            model.Should().BeEquivalentTo(service.GetSubmitModel(userTree.Id , currency).Result);
        }

        [Test]
        public void WhenGetTotalPriceShouldReturnTotalPrice()
        {
            var service = serviceProvider.GetService<ICartService>();
            string iso = "BGN";
            var BGNPrice = userOne.Shop.Products.First().Price;
            var USDPrice = userTwo.Shop.Products.First().Price;
            var EURPrice = 0;
            decimal totalPrice = 0;
            if (iso == ServiceConstants.BGN)
            {
                totalPrice += BGNPrice;
                totalPrice += USDPrice / ServiceConstants.BGN_TO_USD;
                totalPrice += EURPrice / ServiceConstants.BGN_TO_EUR;
            }
            else if (iso == ServiceConstants.USD)
            {
                totalPrice += USDPrice;
                totalPrice += BGNPrice * ServiceConstants.BGN_TO_USD;
                totalPrice += EURPrice * ServiceConstants.EUR_TO_USD;
            }
            else if (iso == ServiceConstants.EUR)
            {
                totalPrice += EURPrice;
                totalPrice += USDPrice / ServiceConstants.EUR_TO_USD;
                totalPrice += BGNPrice * ServiceConstants.BGN_TO_EUR;
            }

            totalPrice = Math.Round(totalPrice, 2);

            var result = service.GetTotalPrice(BGNPrice, USDPrice, EURPrice, iso).Result;

            Assert.AreEqual(totalPrice, result);
        }

        [Test]
        public void WhenGetAllCurrenciesShouldReturnThemCorrect()
        {
            var service = serviceProvider.GetService<ICartService>();
            var allCurrencies = new List<Currency>();

            allCurrencies.Add(userOne.Shop.Currency);
            allCurrencies.Add(userTwo.Shop.Currency);

            allCurrencies.Should().BeEquivalentTo(service.GetAllCurrencies().Result.ToList());
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
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "PeshoShop",
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Puche",
                            Category = new Category()
                            {
                                Name = "Animals"
                            },
                            Image = new byte[20],
                            Price = 20
                        }
                    },
                    Currency = new Currency()
                    {
                        Iso = "USD"
                    }
                }
            };

            userTree = new MyArtPlaceUser()
            {
                UserName = "Georgi",
                Email = "max.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.AddAsync(userTree);
            await repo.SaveChangesAsync();
        }
    }
}
