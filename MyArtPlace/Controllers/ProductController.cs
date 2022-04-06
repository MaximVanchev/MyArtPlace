using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Models.Product;
using MyArtPlace.Core.Services;
using System.IO;
using System.Security.Claims;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class ProductController : BaseController
    {
        private readonly IProductService productService;

        public ProductController(IProductService _productService)
        {
            productService = _productService;
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateProduct()
        {
            await CheckMessages();

            var model = new ProductViewModel();

            string defaultImage = @"./wwwroot/corona_bootstrap/images/default_product_image.png";

            using (var stream = System.IO.File.OpenRead(defaultImage))
            {
                var image = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);

                    model.ImageByteArray = memoryStream.ToArray();
                }
            }

            model.AllCategories = await productService.GetAllCategories();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await productService.AddProduct(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful created product!");
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return Redirect("/");
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> MyProducts()
        {
            await CheckMessages();

            try
            {
                var model = await productService.GetUserProducts(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
                return Redirect("/");
            }
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProduct(Guid productId)
        {
            await CheckMessages();

            try
            {
                var model = await productService.GetProductForEdit(productId , User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
            catch(ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(MyProducts));
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> EditProduct(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await productService.EditProduct(model);
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful saved changes!");
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(MyProducts));
        }

        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProductQuestion(Guid productId)
        {
            await CheckMessages();

            ViewData["ProductId"] = productId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            try
            {
                await productService.DeleteProduct(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful deleted product!");
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(MyProducts));
        }

        [HttpPost]
        public async Task<IActionResult> LikeProduct(Guid productId)
        {
            try
            {
                await productService.LikeProduct(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful liked product!");
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> DislikeProduct(Guid productId)
        {
            try
            {
                await productService.DislikeProduct(productId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                MessageViewModel.Message.Add(MessageConstants.SuccessMessage, "Successful disliked product!");
            }
            catch (ArgumentException aex)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, aex.Message);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return RedirectToAction(nameof(FavoritesProducts));
        }

        public async Task<IActionResult> FavoritesProducts()
        {
            await CheckMessages();

            try
            {
                var model = await productService.GetUserFavoritesProducts(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
                return Redirect("/");
            }
        }

        public async Task<IActionResult> ProductDetails(Guid productId)
        {
            await CheckMessages();

            try
            {
                var model = await productService.GetProductDetails(productId);
                return View(model);
            }
            catch (Exception)
            {
                MessageViewModel.Message.Add(MessageConstants.ErrorMessage, "There was an error!");
            }

            return Redirect("/");
        }
    }
}
