using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Core.Contracts
{
    public interface ICategoryService
    {
        Task<bool> DeleteCategoryById(Guid id);

        Task<Category> GetCategoryById(Guid id);

        Task<IEnumerable<CategoryListViewModel>> AllCategories();

        Task<bool> AddCategory(AddCategoryViewModel model);
    }
}
