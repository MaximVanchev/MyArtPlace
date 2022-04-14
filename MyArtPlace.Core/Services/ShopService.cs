using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<MyArtPlaceUser> userManager;

        public ShopService(IApplicationDbRepository _repo , UserManager<MyArtPlaceUser> _userManager)
        {
            repo = _repo;
            userManager = _userManager;
        }

        public async Task CreateShop(ShopViewModel model, string userId)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(userId);

            if (user == null)
            {
                throw new Exception();
            }

            var shop = new Shop()
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                User = user,
                Currency = await GetCurrencyByIso(model.Currency),
            };

            await repo.AddAsync(shop);

            if (userManager != null)
            {
                await userManager.AddToRoleAsync(user, "Seller");
            }

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Currency>> GetAllCurrencies()
        {
            return await repo.All<Currency>().ToListAsync();
        }

        public async Task<Currency> GetCurrencyByIso(string Iso)
        {
            return await repo.All<Currency>().FirstOrDefaultAsync(c => c.Iso == Iso);
        }

        public async Task<ShopEditViewModel> GetShopForEdit(string userId)
        {
            var user = await repo.All<MyArtPlaceUser>().Include(u => u.Shop).ThenInclude(s => s.Currency).FirstOrDefaultAsync(u => u.Id == userId);

            return new ShopEditViewModel()
            {
                Name = user.Shop.Name,
                Currency = user.Shop.Currency.Iso,
                Location = user.Shop.Location,
                Description = user.Shop.Description
            };
        }

        public async Task EditShop(ShopEditViewModel model , string userId)
        {
            var user = await repo.All<MyArtPlaceUser>().Include(u => u.Shop).FirstOrDefaultAsync(u => u.Id == userId);

            user.Shop.Name = model.Name;
            user.Shop.Currency = await GetCurrencyByIso(model.Currency);
            user.Shop.Location = model.Location;
            user.Shop.Description = model.Description;

            await repo.SaveChangesAsync();
        }
    }
}
