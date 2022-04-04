using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Services;

namespace MyArtPlace.Controllers
{ 
    [Authorize(Roles = "Admin")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService _categoryService)
        {
            categoryService = _categoryService;
        }

        public async Task<IActionResult> ManageCategories()
        {
            await CheckMessages();

            var categories = await categoryService.AllCategories();


            return View(categories);
        }

        public async Task<IActionResult> Add()
        {
            var category = new AddCategoryViewModel();

            await CheckMessages();

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
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Succsessful added category!");
            }
            else
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(ManageCategories));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {

            if (await categoryService.DeleteCategoryById(id))
            {
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Succsessful deleted category!");
            }
            else
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(ManageCategories), ViewData);
        }
    }
}
