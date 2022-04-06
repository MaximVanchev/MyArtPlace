using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Contracts;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService cartService;

        public CartController(ICartService _cartService)
        {
            cartService = _cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToCart(Guid productId)
        {
            await CheckMessages();


        }
    }
}
