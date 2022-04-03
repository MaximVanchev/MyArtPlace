using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Models.Shop;
using MyArtPlace.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface IShopService
    {
        Task CreateShop(ShopViewModel model , string userId);

        Task<ShopEditViewModel> ShopEditById(Guid Id);

        Task<IEnumerable<Currency>> GetAllCurrencies();
    }
}
