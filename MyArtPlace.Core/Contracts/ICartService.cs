using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface ICartService
    {
        Task AddProductToCart(Guid productId, string userId);

        Task RemoveProductFromCart(Guid cartId, string userId);

        Task IncreaseProductCount(Guid cartId, string userId);

        Task DecreaseProductCount(Guid cartId, string userId);

        Task SubmitOrder(CartAddressSubmitViewModel model, string userId);

        Task<CartListViewModel> GetUserCart(string userId);

        Task<CartAddressSubmitViewModel> GetSubmitModel(string userId, string iso);

        Task<IEnumerable<Currency>> GetAllCurrencies();

        Task<decimal> GetTotalPrice(decimal BGNPrice, decimal USDPrice, decimal EURPrice, string iso);
    }
}
