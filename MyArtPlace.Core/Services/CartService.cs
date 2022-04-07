using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class CartService : ICartService
    {
        private readonly IApplicationDbRepository repo; 
        private readonly IShopService shopService;
        private readonly IMailService mailService;

        public CartService(IApplicationDbRepository _repo, IShopService _shopService , IMailService _mailService)
        {
            repo = _repo;
            shopService = _shopService;
            mailService = _mailService;
        }

        public async Task AddProductToCart(Guid productId, string userId)
        {
            var user = await repo.All<MyArtPlaceUser>()
                .Include(u => u.CartProducts)
                .Include(u => u.Shop)
                .ThenInclude(s => s.Products)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var product = await repo.GetByIdAsync<Product>(productId);

            var cart = await repo.All<Cart>().Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.ProductId == productId);

            if (cart != null)
            {
                throw new ArgumentException("You already added this product to your cart!");
            }
            else if (user.Shop.Products.Contains(product))
            {
                throw new ArgumentException("You can't add your product to cart!");
            }

            cart = new Cart 
            { 
                User = user,
                Product = product
            };

            await repo.AddAsync(cart);

            await repo.SaveChangesAsync();
        }

        public async Task RemoveProductFromCart(Guid cartId , string userId)
        {
            var cart = await repo.All<Cart>().Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.OrderId == cartId);

            if (cart == null)
            {
                throw new ArgumentException("There is no product to remove!");
            }

            await repo.DeleteAsync<Cart>(cart.OrderId);

            await repo.SaveChangesAsync();
        }

        public async Task IncreaseProductCount(Guid cartId, string userId)
        {
            var cart = await repo.All<Cart>().Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.OrderId == cartId);

            if (cart == null)
            {
                throw new ArgumentException("There is no product to increase!");
            }
            else if (cart.ProductConut == 10)
            {
                throw new ArgumentException("You can't buy more than 10 at a time!");
            }

            cart.ProductConut++;

            await repo.SaveChangesAsync();
        }

        public async Task DecreaseProductCount(Guid cartId, string userId)
        {
            var cart = await repo.All<Cart>().Where(c => c.UserId == userId).FirstOrDefaultAsync(c => c.OrderId == cartId);

            if (cart == null)
            {
                throw new ArgumentException("There is no product to decrease!");
            }
            else if (cart.ProductConut == 1)
            {
                throw new ArgumentException("You can't buy less then 1!");
            }

            cart.ProductConut--;

            await repo.SaveChangesAsync();
        }

        public async Task SubmitOrder(CartAddressSubmitViewModel model , string userId)
        {

            var user = await repo.All<MyArtPlaceUser>()
                .Include(u => u.CartProducts.Where(c => c.InCart == true))
                .ThenInclude(c => c.Product)
                .ThenInclude(p => p.Shop)
                .ThenInclude(s => s.Currency)
                .FirstOrDefaultAsync(u => u.Id == userId);

            foreach (var cartProduct in user.CartProducts)
            {
                var productUser = await repo.All<MyArtPlaceUser>()
                    .Include(u => u.Shop)
                    .ThenInclude(s => s.Products)
                    .FirstOrDefaultAsync(u => u.Shop.Products.Contains(cartProduct.Product));
                StringBuilder sb = new StringBuilder();
                cartProduct.InCart = false;
                sb.AppendLine($"You have an order from {user.UserName}.");
                sb.AppendLine($"The buyer email is {user.Email}.");
                sb.AppendLine($"Order address is : {model.OrederAddress}.");
                sb.AppendLine($"The order is :");
                sb.AppendLine($"Product name : {cartProduct.Product.Name}.");
                sb.AppendLine($"Product price : {cartProduct.Product.Price} {cartProduct.Product.Shop.Currency.Iso}.");
                sb.AppendLine($"Product count : {cartProduct.ProductConut}.");

                //Sends email to the product creator.
                //await mailService.SendEmailWithMessageAsync(productUser.Email, productUser.UserName, sb.ToString());
            }
            await repo.SaveChangesAsync();
        }

        public async Task<CartListViewModel> GetUserCart(string userId)
        {
            var user = await repo.All<MyArtPlaceUser>()
                .Include(u => u.CartProducts.Where(c => c.InCart == true))
                .ThenInclude(c => c.Product)
                .ThenInclude(p => p.Shop)
                .ThenInclude(p => p.Currency)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var cartList = new CartListViewModel()
            {
                CartProducts = user.CartProducts.Select(p => new CartViewModel()
                {
                    CartId = p.OrderId,
                    ProductId = p.ProductId,
                    ProductName = p.Product.Name,
                    ProductPrice = p.Product.Price,
                    ProductIso = p.Product.Shop.Currency.Iso,
                    ProductCount = p.ProductConut
                }).ToList(),
                AllCurrencies = await shopService.GetAllCurrencies()
            };

            return cartList;
        }

        public async Task<CartAddressSubmitViewModel> GetSubmitModel(string userId , string iso)
        {
            var user = await repo.All<MyArtPlaceUser>()
                .Include(u => u.CartProducts.Where(c => c.InCart == true))
                .ThenInclude(c => c.Product)
                .ThenInclude(p => p.Shop)
                .ThenInclude(s => s.Currency)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user.CartProducts.Count() == 0)
            {
                throw new ArgumentException("You dont have products in the cart!");
            }

            decimal BGNPrice = 0;
            decimal EURPrice = 0;
            decimal USDPrice = 0;

            foreach (var cartProduct in user.CartProducts)
            {
                if (cartProduct.Product.Shop.Currency.Iso == ServiceConstants.BGN)
                {
                    BGNPrice += cartProduct.ProductConut * cartProduct.Product.Price;
                }
                else if (cartProduct.Product.Shop.Currency.Iso == ServiceConstants.EUR)
                {
                    EURPrice += cartProduct.ProductConut * cartProduct.Product.Price;
                }
                else if (cartProduct.Product.Shop.Currency.Iso == ServiceConstants.USD)
                {
                    USDPrice += cartProduct.ProductConut * cartProduct.Product.Price;
                }
            }

            decimal totalPrice = await GetTotalPrice(BGNPrice, USDPrice, EURPrice, iso);

            return new CartAddressSubmitViewModel()
            {
                Currency = iso,
                TotalPrice = totalPrice
            };
        }

        private async Task<decimal> GetTotalPrice(decimal BGNPrice , decimal USDPrice , decimal EURPrice , string iso)
        {
            decimal totalPrice = 0;

            if (iso == ServiceConstants.BGN)
            {
                totalPrice += BGNPrice;
                totalPrice += USDPrice / ServiceConstants.BGN_TO_USD;
                totalPrice += EURPrice / ServiceConstants.BGN_TO_EUR;
            }
            else if (iso == ServiceConstants.USD)
            {
                totalPrice += USDPrice;
                totalPrice += BGNPrice * ServiceConstants.BGN_TO_USD;
                totalPrice += EURPrice * ServiceConstants.EUR_TO_USD;
            }
            else if (iso == ServiceConstants.EUR)
            {
                totalPrice += EURPrice;
                totalPrice += USDPrice / ServiceConstants.EUR_TO_USD;
                totalPrice += BGNPrice * ServiceConstants.BGN_TO_EUR;
            }

            return Math.Round(totalPrice , 2);
        }
    }
}
