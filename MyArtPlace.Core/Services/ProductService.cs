using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Product;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IApplicationDbRepository repo;

        public ProductService(IApplicationDbRepository _repo)
        {
            repo = _repo;
        }

        public async Task AddProduct(ProductViewModel model , string userId)
        {
            var user = await repo.All<MyArtPlaceUser>().Include(u => u.Shop).FirstOrDefaultAsync(u => u.Id == userId);

            var product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Shop = user.Shop,
                Category = await GetCategoryByName(model.Category),
                Price = model.Price
            };

            if (model.Image == null)
            {
                throw new ArgumentException(MessageConstants.ImageIsNullErrorMessage);
            }

            if (model.ImageByteArray == null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(memoryStream);

                    product.Image = memoryStream.ToArray();
                }
            }

            await repo.AddAsync(product);

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await repo.All<Category>().ToListAsync();
        }

        public async Task EditProduct(ProductEditViewModel model)
        {
            var product = await repo.GetByIdAsync<Product>(model.Id);

            product.Name = model.Name;
            product.Description = model.Description;
            product.Category = await GetCategoryByName(model.Category);
            product.Price = model.Price;


            if (model.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.Image.CopyToAsync(memoryStream);

                    product.Image = memoryStream.ToArray();
                }
            }

            await repo.SaveChangesAsync();

        }

        public async Task<Category> GetCategoryByName(string name)
        {
            return await repo.All<Category>().FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<ProductEditViewModel> GetProductForEdit(Guid productId , string userId)
        {
            var product = await repo.All<Product>()
                .Include(p => p.Category)
                .Include(p => p.Shop)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product.Shop.User.Id != userId)
            {
                throw new ArgumentException(MessageConstants.UserDontHaveProductForEditErrorMessage);
            }

            return new ProductEditViewModel()
            {
                Name = product.Name,
                Category = product.Category.Name,
                Description = product.Description,
                Price = product.Price,
                Id = product.Id,
                ImageByteArray  = product.Image,
                AllCategories = await GetAllCategories()
            };
        }

        public async Task<IEnumerable<ProductListViewModel>> GetUserProducts(string userId)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(userId);

            if (user == null)
            {
                throw new Exception();
            }

            var products = await repo.All<Product>()
                .Include(p => p.Category)
                .Include(p => p.Shop)
                .ThenInclude(s => s.Currency)
                .Where(p => p.Shop.User == user)
                .ToListAsync();

            return products.Select(p => new ProductListViewModel
            {
                Name = p.Name,
                Category = p.Category.Name,
                Id = p.Id,
                ImageByteArray = p.Image,
                Price = p.Price,
                Iso = p.Shop.Currency.Iso,
                Likes = p.UsersLiked.Count()
            });
        }

        public async Task<IEnumerable<ProductListViewModel>> GetAllProducts(string userId)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(userId);

            var products = await repo.All<Product>()
                .Include(p => p.Category)
                .Include(p => p.UsersLiked)
                .Include(p => p.Shop)
                .ThenInclude(s => s.Currency)
                .ToListAsync();

            return products.Select(p => new ProductListViewModel()
            {
                Name = p.Name,
                Category = p.Category.Name,
                Id = p.Id,
                ImageByteArray = p.Image,
                Likes= p.UsersLiked.Count(),
                Price = p.Price,
                Iso = p.Shop.Currency.Iso,
                UserLiked = p.UsersLiked.Contains(user)
            });
        }

        public async Task DeleteProduct(Guid productId , string userId)
        {
            var user = await repo.All<MyArtPlaceUser>().Include(u => u.Shop).ThenInclude(s => s.Products).FirstOrDefaultAsync(u => u.Id == userId);

            var product = await repo.GetByIdAsync<Product>(productId);

            if (!user.Shop.Products.Contains(product))
            {
                throw new ArgumentException(MessageConstants.DontHavePermissionToDelete);
            }

            await repo.DeleteAsync<Product>(product.Id);

            await repo.SaveChangesAsync();
        }

        public async Task LikeProduct(Guid productId, string userId)
        {
            var user = await repo.All<MyArtPlaceUser>().Include(u => u.Shop).ThenInclude(s => s.Products).FirstOrDefaultAsync(u => u.Id == userId);

            var product = await repo.GetByIdAsync<Product>(productId);

            if (user.Shop.Products.Contains(product))
            {
                throw new ArgumentException(MessageConstants.CantLikeYourProductErrorMessage);
            }

            user.LikedProducts.Add(product);

            await repo.SaveChangesAsync();
        }

        public async Task DislikeProduct(Guid productId, string userId)
        {
            var user = await repo.All<MyArtPlaceUser>()
                .Include(u => u.Shop)
                .ThenInclude(s => s.Products)
                .Include(u => u.LikedProducts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            var product = await repo.GetByIdAsync<Product>(productId);

            if (user.Shop.Products.Contains(product))
            {
                throw new ArgumentException(MessageConstants.CantDislikeYourProductErrorMessage);
            }

            user.LikedProducts.Remove(product);

            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductListViewModel>> GetUserFavoritesProducts(string userId)
        {
            var user = await repo.GetByIdAsync<MyArtPlaceUser>(userId);

            if (user == null)
            {
                throw new Exception();
            }

            var products = await repo.All<Product>()
                .Include(p => p.Category)
                .Include(p => p.UsersLiked)
                .ThenInclude(u => u.Shop)
                .ThenInclude(s => s.Currency)
                .Where(p => p.UsersLiked.Contains(user))
                .Select(p => new ProductListViewModel
                {
                    Name = p.Name,
                    Category = p.Category.Name,
                    Id = p.Id,
                    ImageByteArray = p.Image,
                    Price = p.Price,
                    Iso = p.Shop.Currency.Iso,
                    Likes = p.UsersLiked.Count()
                })
                .ToListAsync();

            return products;
        }

        public async Task<ProductDetailsViewModel> GetProductDetails(Guid productId)
        {
            var product = await repo.All<Product>()
                .Include(p => p.Category)
                .Include(p => p.Shop)
                .ThenInclude(s => s.Currency)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return new ProductDetailsViewModel()
            {
                Name = product.Name,
                Category = product.Category.Name,
                Description = product.Description,
                Price = product.Price,
                ImageByteArray = product.Image,
                Currency = product.Shop.Currency.Iso
            };
        }
    }
}
