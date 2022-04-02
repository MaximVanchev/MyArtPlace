using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IApplicationDbRepository repo;

        public CurrencyService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task<bool> AddCurrency(AddCurrencyViewModel model)
        {
            Currency currency = new Currency()
            {
                Iso = model.Iso
            };

            try
            {
                await repo.AddAsync(currency);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<CurrencyListViewModel>> AllCurrencies()
        {
            return await repo.All<Currency>()
            .Select(x => new CurrencyListViewModel
            {
                Id = x.Id,
                Iso = x.Iso
            }).ToListAsync();
        }

        public async Task<bool> DeleteCurrencyById(Guid id)
        {
            try
            {
                await repo.DeleteAsync<Currency>(id);
                await repo.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Currency> GetCurrencyById(Guid id)
        {
            return await repo.GetByIdAsync<Currency>(id);
        }
    }
}
