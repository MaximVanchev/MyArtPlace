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
    public class UserControllerTests
    {
        private FakeSignInManager<MyArtPlaceUser> FakeSignInManager;
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private UserController userController;
        private UserController userControllerErrorUser;
        private MyArtPlaceUser user;

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

            userController.CheckMessages();
        }

        [Test]
        public void WhenGetSettingsShouldReturnView()
        {
            var result = userController.Settings().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<ProfileEditViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetSettingsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var result = userControllerErrorUser.Settings().Result as RedirectResult;
            Assert.AreEqual(result.Url , "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostSettingsShouldRedirectToIndexAndEditUser()
        {
            var model = new ProfileEditViewModel()
            {
                Id = user.Id,
                Email = "adhgs@gmail.com",
                Username = "Koko",
            };

            var result = userController.Settings(model).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(model.Email, user.Email);
            Assert.AreEqual(model.Username, user.UserName);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage] , MessageConstants.SuccessfulSavedChanges);
        }

        [Test]
        public void WhenPostSettingsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var model = new ProfileEditViewModel();

            var result = userControllerErrorUser.Settings(model).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostSettingsAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ProfileEditViewModel()
            {
                Id = user.Id,
                Email = "adhgs@gmail.com",
                Username = "Koko",
            };

            userController.ModelState.AddModelError("error", "error message");

            var result = userController.Settings(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [TearDown]
        public void TearDown()
        {
            userController.CheckMessages();

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

            var userService = new UserService(repo , FakeSignInManager);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));

            var userErrorClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            userController = new UserController(userService);
            userControllerErrorUser = new UserController(userService);

            userController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            userControllerErrorUser.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userErrorClaims }
            };

            await repo.AddAsync(user);
            await repo.SaveChangesAsync();
        }
    }
}
