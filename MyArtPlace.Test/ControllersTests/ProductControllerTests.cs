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

namespace MyArtPlace.Test.ControllersTests
{
    public class ProductControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private ProductController productControllerUserOne;
        private ProductController productControllerUserTwo;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenPostCreateProductAndThereIsArgumentExShouldReturnRedirectToIndexAndAddMessage()
        {
            var model = new ProductViewModel()
            {
                Category = "Prints",
                Description = "dsad",
                Name = "Toyasd",
                Price = 100
            };
            var result = productControllerUserTwo.CreateProduct(model).Result as RedirectResult;
            Assert.NotNull(result);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ImageIsNullErrorMessage);
        }

        [Test]
        public void WhenPostCreateProductAndThereIsErrorShouldReturnRedirectToIndexAndAddMessage()
        {
            var model = new ProductViewModel();
            var result = productControllerUserOne.CreateProduct(model).Result as RedirectResult;
            Assert.NotNull(result);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostCreateProductAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ProductViewModel();

            productControllerUserTwo.ModelState.AddModelError("error", "error message");

            var result = productControllerUserTwo.CreateProduct(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [TearDown]
        public void TearDown()
        {
            productControllerUserTwo.CheckMessages();

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

            var productService = serviceProvider.GetService<IProductService>();

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

            productControllerUserOne = new ProductController(productService);
            productControllerUserTwo = new ProductController(productService);

            productControllerUserTwo.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userTwoClaims }
            };

            productControllerUserOne.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userOneClaims }
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.SaveChangesAsync();
        }
    }
}
