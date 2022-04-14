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
    public class CategoryControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private CategoryController categoryController;
        private Category category;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICategoryService , CategoryService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);

            categoryController.CheckMessages();
        }

        [Test]
        public void WhenGetManageCategoriesShouldReturnView()
        {
            var result = categoryController.ManageCategories().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<IEnumerable<CategoryListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetAddShouldRetutnView()
        {
            var result = categoryController.Add().Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<AddCategoryViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenPostAddShouldRedirectToActionAndAddCategory()
        {
            var model = new AddCategoryViewModel()
            {
                Name = "Kartini"
            };
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var categories = repo.All<Category>();

            var result = categoryController.Add(model).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(categoryController.ManageCategories));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulAddedCategoryMessage);
            Assert.True(categories.Count() == 2);
        }

        [Test]
        public void WhenPostAddAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var model = new AddCategoryViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var categories = repo.All<Category>();

            var result = categoryController.Add(model).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(categoryController.ManageCategories));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostDeleteAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var model = new AddCategoryViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var categories = repo.All<Category>();

            var result = categoryController.Delete(Guid.NewGuid()).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(categoryController.ManageCategories));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostAddAndModelStateIsNotValidShouldReturnViewAndAddMessage()
        {
            var model = new AddCategoryViewModel();
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var categories = repo.All<Category>();

            categoryController.ModelState.AddModelError("error", "error message");

            var result = categoryController.Add(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenPostDeleteShouldRedirectToActionAndDeleteCategory()
        {
            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            var categories = repo.All<Category>();

            var result = categoryController.Delete(category.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(categoryController.ManageCategories));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulDeletedCategoryMessage);
            Assert.True(categories.Count() == 0);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            category = new Category()
            {
                Name = "Prints"
            };

            var categoryService = serviceProvider.GetService<ICategoryService>();

            categoryController = new CategoryController(categoryService);

            await repo.AddAsync(category);
            await repo.SaveChangesAsync();
        }
    }
}
