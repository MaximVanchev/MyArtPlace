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
    }
}
