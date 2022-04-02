using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface ICurrencyService
    {
        Task<Currency> GetCurrencyById(Guid id);

        Task<IEnumerable<CurrencyListViewModel>> AllCurrencies();

        Task<bool> AddCurrency(AddCurrencyViewModel model);

        Task<bool> DeleteCurrencyById(Guid id);
    }
}
