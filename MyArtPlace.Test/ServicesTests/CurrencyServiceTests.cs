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
    public class CurrencyServiceTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private Currency currencyOne;
        private Currency currencyTwo;

        [SetUp]
        public async Task SetUp()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();

            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<ICurrencyService, CurrencyService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);
        }

        [Test]
        public void WhenAdminAddCurrencyShouldBeAdded()
        {
            var service = serviceProvider.GetService<ICurrencyService>();
            var model = new AddCurrencyViewModel()
            {
                Iso = "EUR"
            };

            Assert.True(service.AddCurrency(model).Result);
            Assert.True(service.AllCurrencies().Result.Count() == 3);
        }

        [Test]
        public void WhenAdminAddCurrencyWithNoNameMustThrow()
        {
            var service = serviceProvider.GetService<ICurrencyService>();
            var model = new AddCurrencyViewModel()
            {
                Iso = null
            };

            Assert.False(service.AddCurrency(model).Result);
            Assert.True(service.AllCurrencies().Result.Count() == 2);
        }

        [Test]
        public void WhenGetAllCurrenciesShouldReturnAllCurrencies()
        {
            var service = serviceProvider.GetService<ICurrencyService>();
            var model = new CurrencyListViewModel()
            {
                Id = currencyOne.Id,
                Iso = currencyOne.Iso,
            };

            Assert.True(service.AllCurrencies().Result.Count() == 2);
            model.Should().BeEquivalentTo(service.AllCurrencies().Result.First(x => x.Id == currencyOne.Id));
        }

        [Test]
        public void WhenAdminDeleteCurrencyShouldBeDeleted()
        {
            var service = serviceProvider.GetService<ICurrencyService>();

            Assert.True(service.DeleteCurrencyById(currencyOne.Id).Result);
            Assert.True(service.AllCurrencies().Result.Count() == 1);
        }

        [Test]
        public void WhenAdminDeleteCurrencyWithWrongIdMustThrow()
        {
            var service = serviceProvider.GetService<ICurrencyService>();

            Assert.False(service.DeleteCurrencyById(Guid.NewGuid()).Result);
            Assert.True(service.AllCurrencies().Result.Count() == 2);
        }

        [Test]
        public void WhenGetCurrencyByIdShouldReturnCurrency()
        {
            var service = serviceProvider.GetService<ICurrencyService>();

            currencyOne.Should().BeEquivalentTo(service.GetCurrencyById(currencyOne.Id).Result);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            currencyOne = new Currency()
            {
                Iso = "BGN"
            };

            currencyTwo = new Currency()
            {
                Iso = "USD"
            };

            await repo.AddAsync(currencyOne);
            await repo.AddAsync(currencyTwo);
            await repo.SaveChangesAsync();
        }
    }
}
