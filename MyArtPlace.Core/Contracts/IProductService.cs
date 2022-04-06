using MyArtPlace.Core.Models.Product;
using MyArtPlace.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface IProductService
    {
        Task AddProduct(ProductViewModel model , string userId);

        Task<IEnumerable<Category>> GetAllCategories();

        Task<ProductEditViewModel> GetProductForEdit(Guid productId , string userId);

        Task EditProduct(ProductEditViewModel model);

        Task<Category> GetCategoryByName(string name);

        Task<IEnumerable<ProductListViewModel>> GetUserProducts(string userId);

        Task<IEnumerable<ProductListViewModel>> GetAllProducts(string userId);

        Task<IEnumerable<ProductListViewModel>> GetUserFavoritesProducts(string userId);

        Task DeleteProduct(Guid productId , string userId);

        Task LikeProduct(Guid productId, string userId);

        Task DislikeProduct(Guid productId, string userId);

        Task<ProductDetailsViewModel> GetProductDetails(Guid productId);
    }
}
