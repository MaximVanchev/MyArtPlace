using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Shop;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class ShopService : IShopService
    {
        private readonly IApplicationDbRepository repo;

        public ShopService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task CreateShop(ShopViewModel model, string userId)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(userId);

            var shop = new Shop()
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                User = user,
                Currency = model.Currency,
            };

            await repo.AddAsync(shop);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            return await repo.All<Currency>().ToListAsync();
        }

        public Task<ShopEditViewModel> ShopEditById(Guid Id)
        {
            throw new NotImplementedException();
        }
    }
}
