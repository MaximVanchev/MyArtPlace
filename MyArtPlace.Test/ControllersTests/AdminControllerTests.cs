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
using MyArtPlace.Core.Constants;

namespace MyArtPlace.Test.ControllersTests
{
    public class AdminControllerTests
    {
        private FakeRoleManager<IdentityRole> roleManager;
        private FakeUserManager<MyArtPlaceUser> userManager;
        private FakeSignInManager<MyArtPlaceUser> signInManager;
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private AdminController adminController;
        private MyArtPlaceUser userOne;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            roleManager = new FakeRoleManager<IdentityRole>();
            userManager = new FakeUserManager<MyArtPlaceUser>();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IApplicationDbRepository , ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenGetRolesShouldReturnView()
        {
            var result = (ViewResult)adminController.Roles(userOne.Id).Result;
            Assert.NotNull(result);
        }

        [Test]
        public void WhenGetRolesShouldReturnViewModel()
        {
            var model = new UserRolesViewModel()
            {
                Id = userOne.Id,
                Username = userOne.UserName
            };

            var result = (ViewResult)adminController.Roles(userOne.Id).Result;
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenGetManageUsersShouldReturnView()
        {
            var model = new UserRolesViewModel()
            {
                Id = userOne.Id,
                RoleNames = new string[0],
                Username = userOne.UserName
            };

            var result = (ViewResult)adminController.ManageUsers().Result;
            Assert.NotNull(result);
        }

        [Test]
        public void WhenGetManageUsersShouldReturnCorrectModel()
        {
            var model = new UserRolesViewModel()
            {
                Id = userOne.Id,
                RoleNames = new string[0],
                Username = userOne.UserName
            };

            var result = (ViewResult)adminController.ManageUsers().Result;
            result.Model.ShouldBe<IEnumerable<UserListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenPostCreateRoleShouldRedirectToIndex()
        {
            var model = new CreateRoleViewModel()
            {
                Name = "Seller"
            };

            var result = (RedirectResult)adminController.CreateRole(model).Result;
            Assert.AreEqual(result.Url, "/");
        }

        [Test]
        public void WhenPostCreateRoleWithNotValidStateShouldReturnView()
        {
            var model = new CreateRoleViewModel()
            {
                Name = "Seller"
            };

            adminController.ModelState.AddModelError("error", "error message");

            var result = adminController.CreateRole(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [TearDown]
        public void TearDown()
        {
            adminController.CheckMessages();

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

            var userService = new UserService(repo, signInManager);

            adminController = new AdminController(roleManager, userManager, userService);

            await repo.AddAsync(userOne);
            await repo.SaveChangesAsync();
        }
    }
}
