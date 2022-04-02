using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Services;

namespace MyArtPlace.Controllers
{ 
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        public async Task<IActionResult> ManageCategories()
        {
            var categories = await categoryService.AllCategories();

            return View(categories);
        }

        public async Task<IActionResult> Add()
        {
            var category = new AddCategoryViewModel();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await categoryService.AddCategory(model))
            {
                ViewData[MessageConstants.SuccessMessage] = "Successful added category!";
            }
            else
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return RedirectToAction(nameof(ManageCategories) , ViewData);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {

            if (await categoryService.DeleteCategoryById(id))
            {
                ViewData[MessageConstants.SuccessMessage] = "Succsessful deleted category!";
            }
            else
            {
                ViewData[MessageConstants.ErrorMessage] = "There was an error!";
            }

            return RedirectToAction(nameof(ManageCategories), ViewData);
        }
    }
}
