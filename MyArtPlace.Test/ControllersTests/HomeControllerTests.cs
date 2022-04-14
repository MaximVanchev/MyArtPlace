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
using MyArtPlace.Models;

namespace MyArtPlace.Test.ControllersTests
{
    public class HomeControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private HomeController homeController;
        private MyArtPlaceUser user;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IProductService , ProductService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);

            homeController.CheckMessages();
        }

        [Test]
        public void WhenGetIndexShouldReturnView()
        {
            var result = homeController.Index().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<IEnumerable<ProductListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetErrorShouldReturnView()
        {
            var result = homeController.Error().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<ErrorViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [TearDown]
        public void TearDown()
        {
            homeController.CheckMessages();

            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            user = new MyArtPlaceUser()
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

            var productService = serviceProvider.GetService<IProductService>();

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));

            homeController = new HomeController(productService);

            homeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            await repo.AddAsync(user);
            await repo.SaveChangesAsync();
        }
    }
}
