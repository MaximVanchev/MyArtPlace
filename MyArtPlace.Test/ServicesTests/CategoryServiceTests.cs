using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Services;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Test.ServicesTests
{
    public class CategoryServiceTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private Category categoryOne;
        private Category categoryTwo;

        [SetUp]
        public async Task SetUp()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICategoryService, CategoryService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenAdminAddCategoryShouldBeAdded()
        {
            var service = serviceProvider.GetService<ICategoryService>();
            var model = new AddCategoryViewModel()
            {
                Name = "Kone"
            };

            Assert.True(service.AddCategory(model).Result);
            Assert.True(service.AllCategories().Result.Count() == 3);
        }

        [Test]
        public void WhenAdminAddCategoryWithNoNameMustThrow()
        {
            var service = serviceProvider.GetService<ICategoryService>();
            var model = new AddCategoryViewModel()
            {
                Name = null
            };

            Assert.False(service.AddCategory(model).Result);
            Assert.True(service.AllCategories().Result.Count() == 2);
        }

        [Test]
        public void WhenGetAllCategoriesShouldReturnAllCategories()
        {
            var service = serviceProvider.GetService<ICategoryService>();
            var model = new CategoryListViewModel()
            {
                Id = categoryOne.Id,
                Name = categoryOne.Name,
            };

            Assert.True(service.AllCategories().Result.Count() == 2);
            model.Should().BeEquivalentTo(service.AllCategories().Result.First(x => x.Id == categoryOne.Id));
        }

        [Test]
        public void WhenAdminDeleteCategoryShouldBeDeleted()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            Assert.True(service.DeleteCategoryById(categoryOne.Id).Result);
            Assert.True(service.AllCategories().Result.Count() == 1);
        }

        [Test]
        public void WhenAdminDeleteCategoryWithWrongIdMustThrow()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            Assert.False(service.DeleteCategoryById(Guid.NewGuid()).Result);
            Assert.True(service.AllCategories().Result.Count() == 2);
        }

        [Test]
        public void WhenGetCategoryByIdShouldReturnCategory()
        {
            var service = serviceProvider.GetService<ICategoryService>();

            categoryOne.Should().BeEquivalentTo(service.GetCategoryById(categoryOne.Id).Result);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            categoryOne = new Category()
            {
                Name = "Prints"
            };

            categoryTwo = new Category()
            {
                Name = "Paintings"
            };

            await repo.AddAsync(categoryOne);
            await repo.AddAsync(categoryTwo);
            await repo.SaveChangesAsync();
        }
    }
}
