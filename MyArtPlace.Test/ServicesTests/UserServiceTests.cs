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

namespace MyArtPlace.Test.ServicesTests
{
    public class UserServiceTests
    {
        FakeSignInManager<MyArtPlaceUser> signInManager;
        private UserService userService;
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;
        private MyArtPlaceUser userTree;

        [SetUp]
        public async Task SetUp()
        {
            signInManager = new FakeSignInManager<MyArtPlaceUser>();
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            userService = new UserService(repo, signInManager);

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenEditUserShouldBeEdited()
        {
            var model = new ProfileEditViewModel()
            {
                Id = userOne.Id,
                Email = "ivan@gmail.com",
                Username = "Ivan"
            };

            userService.EditUser(model);

            Assert.AreEqual(model.Email , userOne.Email);
            Assert.AreEqual(model.Username , userOne.UserName);
        }

        [Test]
        public void WhenGetUserByIdShuldReturnUser()
        {
            userOne.Should().BeEquivalentTo(userService.GetUserById(userOne.Id).Result);
        }

        [Test]
        public void WhenGetUserByUsernameShuldReturnUser()
        {
            userOne.Should().BeEquivalentTo(userService.GetUserByUsername(userOne.UserName).Result);
        }

        [Test]
        public void WhenGetUserForEditWithoutPicturePathShouldReturnUser()
        {
            var model = new ProfileEditViewModel()
            {
                Email = userOne.Email,
                Username = userOne.UserName,
                Id = userOne.Id,
                ImagePath = "../corona_bootstrap/images/users_profile_pictures/avatar_03.png"
            };

            model.Should().BeEquivalentTo(userService.GetUserForEdit(userOne.Id).Result);
        }

        [Test]
        public void WhenGetUserForEditShouldReturnUser()
        {
            var repo = serviceProvider.GetService<IApplicationDbRepository>();
            userOne.ProfilePicture = "picture";
            repo.SaveChanges();
            var model = new ProfileEditViewModel()
            {
                Email = userOne.Email,
                Username = userOne.UserName,
                Id = userOne.Id,
                ImagePath = $"../corona_bootstrap/images/users_profile_pictures/{userOne.ProfilePicture}"
            };

            model.Should().BeEquivalentTo(userService.GetUserForEdit(userOne.Id).Result);
        }

        [Test]
        public void WhenGetUsersShouldReturnUsers()
        {
            var model = new UserListViewModel()
            {
                Username = userOne.UserName,
                Email = userOne.Email,
                Id = userOne.Id
            };

            Assert.True(userService.GetUsers().Result.Count() == 3);
            model.Should().BeEquivalentTo(userService.GetUsers().Result.First(x => x.Id == userOne.Id));
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
