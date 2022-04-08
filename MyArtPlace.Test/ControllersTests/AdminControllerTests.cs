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
using System.Web.Mvc;

namespace MyArtPlace.Test.ControllersTests
{
    public class AdminControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private AdminController adminController;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton <UserManager<MyArtPlaceUser>>()
                .AddSingleton<RoleManager<IdentityRole>>()
                .AddSingleton<IUserService , UserService>()
                .AddSingleton<IApplicationDbRepository , ApplicationDbRepository>()
                .BuildServiceProvider();

            await SeedDbAsync();
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync()
        {
            var releManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetService<UserManager<MyArtPlaceUser>>();
            var userService = serviceProvider.GetService<IUserService>();

            adminController = new AdminController(releManager, userManager, userService);
        }
    }
}
