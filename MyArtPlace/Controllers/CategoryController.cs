using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Services;

namespace MyArtPlace.Controllers
{ 
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }
    }
}
