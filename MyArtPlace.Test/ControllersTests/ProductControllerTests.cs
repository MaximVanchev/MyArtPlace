using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Controllers;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Admin;
using MyArtPlace.Core.Services;
using MyArtPlace.Infrastructure.Data.Repositories;
using NUnit.Framework;
using MvcContrib.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyArtPlace.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using MyArtPlace.Core.Models.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyArtPlace.Core.Models.Cart;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Models.Common;
using MyArtPlace.Core.Models.Product;

namespace MyArtPlace.Test.ControllersTests
{
    public class ProductControllerTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private ProductController productControllerUserOne;
        private ProductController productControllerUserTwo;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;

        [SetUp]
        public async Task Setup()
        {
            dbContext = new InMemoryDbContext();
            var serviceCollection = new ServiceCollection();
            serviceProvider = serviceCollection
                .AddSingleton(sp => dbContext.CreateContext())
                .AddSingleton<IProductService, ProductService>()
                .AddSingleton<IApplicationDbRepository, ApplicationDbRepository>()
                .BuildServiceProvider();

            var repo = serviceProvider.GetService<IApplicationDbRepository>();

            await SeedDbAsync(repo);

            productControllerUserTwo.CheckMessages();
        }

        [Test]
        public void WhenPostCreateProductAndThereIsArgumentExShouldReturnRedirectToIndexAndAddMessage()
        {
            var model = new ProductViewModel()
            {
                Category = "Prints",
                Description = "dsad",
                Name = "Toyasd",
                Price = 100
            };
            var result = productControllerUserTwo.CreateProduct(model).Result as RedirectResult;
            Assert.NotNull(result);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ImageIsNullErrorMessage);
        }

        [Test]
        public void WhenPostCreateProductAndThereIsErrorShouldReturnRedirectToIndexAndAddMessage()
        {
            var model = new ProductViewModel();
            var result = productControllerUserOne.CreateProduct(model).Result as RedirectResult;
            Assert.NotNull(result);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenPostCreateProductAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ProductViewModel();

            productControllerUserTwo.ModelState.AddModelError("error", "error message");

            var result = productControllerUserTwo.CreateProduct(model).Result as ViewResult;
            Assert.NotNull(result);
            model.Should().BeEquivalentTo(result.Model);
        }

        [Test]
        public void WhenGetMyProductsShouldReturnViewWithProducts()
        {
            var result = productControllerUserTwo.MyProducts().Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<IEnumerable<ProductListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
            Assert.True(((IEnumerable<ProductListViewModel>)result.Model).Count() == 1);
        }

        [Test]
        public void WhenGetMyProductsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var result = productControllerUserOne.MyProducts().Result as RedirectResult;
            Assert.NotNull(result);
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenGetEditProductShouldReturnViewWithProduct()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.EditProduct(product.Id).Result as ViewResult;
            Assert.NotNull(result);
            result.Model.ShouldBe<ProductEditViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetEditProductAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.EditProduct(Guid.NewGuid()).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.MyProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [Test]
        public void WhenGetEditProductAndThereIsArgumentExShouldRedirectToIndexAndAddMessage()
        {
            var product = userOne.Shop.Products.First();

            var result = productControllerUserTwo.EditProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.MyProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.UserDontHaveProductForEditErrorMessage);
        }

        [Test]
        public void WhenPostEditProductAndModelStateIsNotValidShouldReturnView()
        {
            var model = new ProductEditViewModel();

            productControllerUserTwo.ModelState.AddModelError("error", "error message");

            var result = productControllerUserTwo.EditProduct(model).Result as ViewResult;
            Assert.NotNull(result);
        }

        [Test]
        public void WhenPostEditProductShouldRedirectToAction()
        {
            var product = userTwo.Shop.Products.First();

            var model = new ProductEditViewModel()
            {
                Name = "Name",
                Category = "Kone",
                Description = "fsadgsa",
                Price = 100,
                Id = product.Id,
            };

            var result = productControllerUserTwo.EditProduct(model).Result as RedirectToActionResult;
            Assert.AreEqual(product.Name, model.Name);
            Assert.AreEqual(product.Description, model.Description);
            Assert.AreEqual(product.Price, model.Price);
            Assert.AreEqual(product.Category.Name, model.Category);
            Assert.AreEqual(result.ActionName, nameof(productControllerUserTwo.MyProducts));
        }

        [Test]
        public void WhenGetDeleteProductQuestionShouldReturnView()
        {
            var product = userOne.Shop.Products.First();

            var result = productControllerUserTwo.DeleteProductQuestion(product.Id).Result as ViewResult;
            Assert.AreEqual(result.ViewData["ProductId"], product.Id);
            Assert.NotNull(result);
        }

        [Test]
        public void WhenPostDeleteProductShouldRedirectToActionAndDelete()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.DeleteProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.MyProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage] , MessageConstants.SuccessfulDeletedProductMessage);
            Assert.True(userTwo.Shop.Products.Count == 0);
        }

        [Test]
        public void WhenPostDeleteProductAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserOne.DeleteProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.MyProducts));
            Assert.NotNull(MessageViewModel.Message[MessageConstants.ErrorMessage]);
        }

        [Test]
        public void WhenPostDeleteProductAndThereIsArgumentErrorShouldRedirectToActionAndAddMessage()
        {
            var product = userOne.Shop.Products.First();

            var result = productControllerUserTwo.DeleteProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.MyProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage] , MessageConstants.DontHavePermissionToDelete);
        }

        [Test]
        public void WhenPostLikeProductShouldRedirectToIndexAndLike()
        {
            var product = userOne.Shop.Products.First();

            var result = productControllerUserTwo.LikeProduct(product.Id).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage] , MessageConstants.SuccsessfulLikeMessage);
            Assert.True(userTwo.LikedProducts.Count == 1);
        }

        [Test]
        public void WhenPostLikeProductAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserOne.LikeProduct(product.Id).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.NotNull(MessageViewModel.Message[MessageConstants.ErrorMessage]);
        }

        [Test]
        public void WhenPostLikeProductAndThereIsArgumentErrorShouldRedirectToIndexAndMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.LikeProduct(product.Id).Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.CantLikeYourProductErrorMessage);
        }

        [Test]
        public void WhenPostDislikeProductShouldRedirectToActionAndDislike()
        {
            var product = userOne.Shop.Products.First();

            productControllerUserTwo.LikeProduct(product.Id);

            productControllerUserTwo.CheckMessages();

            var result = productControllerUserTwo.DislikeProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName , nameof(productControllerUserOne.FavoritesProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.SuccessMessage], MessageConstants.SuccsessfulDislikeMessage);
            Assert.True(userTwo.LikedProducts.Count == 0);
        }

        [Test]
        public void WhenPostDilikeProductAndThereIsErrorShouldRedirectToActionAndAddMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserOne.DislikeProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.FavoritesProducts));
            Assert.NotNull(MessageViewModel.Message[MessageConstants.ErrorMessage]);
        }

        [Test]
        public void WhenPostDislikeProductAndThereIsArgumentErrorShouldRedirectToActionAndMessage()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.DislikeProduct(product.Id).Result as RedirectToActionResult;
            Assert.AreEqual(result.ActionName, nameof(productControllerUserOne.FavoritesProducts));
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.CantDislikeYourProductErrorMessage);
        }

        [Test]
        public void WhenGetProductDetailsShouldReturnView()
        {
            var product = userTwo.Shop.Products.First();

            var result = productControllerUserTwo.ProductDetails(product.Id).Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<ProductDetailsViewModel>(MessageConstants.TestIncorrectTypeReturned);
        }

        [Test]
        public void WhenGetProductDetailsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var result = productControllerUserOne.ProductDetails(Guid.NewGuid()).Result as RedirectResult;
            Assert.AreEqual(result.Url , "/");
            Assert.NotNull(MessageViewModel.Message[MessageConstants.ErrorMessage]);
        }

        [Test]
        public void WhenGetFavoritesProductsShouldReturnView()
        {
            var product = userOne.Shop.Products.First();

            productControllerUserTwo.LikeProduct(product.Id);

            productControllerUserTwo.CheckMessages();

            var result = productControllerUserTwo.FavoritesProducts().Result as ViewResult;
            Assert.IsNotNull(result);
            result.Model.ShouldBe<IEnumerable<ProductListViewModel>>(MessageConstants.TestIncorrectTypeReturned);
            Assert.True(((IEnumerable<ProductListViewModel>)result.Model).Count() == 1);
        }

        [Test]
        public void WhenGetFavoritesProductsAndThereIsErrorShouldRedirectToIndexAndAddMessage()
        {
            var result = productControllerUserOne.FavoritesProducts().Result as RedirectResult;
            Assert.AreEqual(result.Url, "/");
            Assert.AreEqual(MessageViewModel.Message[MessageConstants.ErrorMessage], MessageConstants.ThereWasErrorMessage);
        }

        [TearDown]
        public void TearDown()
        {
            productControllerUserTwo.CheckMessages();

            dbContext.Dispose();
        }

        private async Task SeedDbAsync(IApplicationDbRepository repo)
        {
            userOne = new MyArtPlaceUser()
            {
                UserName = "Maxi",
                Email = "maxim.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "MaxiShop",
                    Currency = new Currency()
                    {
                        Iso = "BGN"
                    },
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Kotka",
                            Category = new Category()
                            {
                                Name = "Prints"
                            },
                            Image = new byte[20],
                            Price = 30
                        },
                        new Product()
                        {
                            Name = "Kon",
                            Category = new Category()
                            {
                                Name = "Kone"
                            },
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            userTwo = new MyArtPlaceUser()
            {
                UserName = "Ivo",
                Email = "max.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                CartProducts = new List<Cart>()
                {
                    new Cart()
                    {
                        Product = userOne.Shop.Products.First(x => x.Name == "Kotka")
                    }
                },
                Shop = new Shop()
                {
                    Name = "IvoShop",
                    Currency = new Currency()
                    {
                        Iso = "USD"
                    },
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Mishka",
                            Category = new Category()
                            {
                                Name = "Mishki"
                            },
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            var productService = serviceProvider.GetService<IProductService>();

            var userTwoClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, userTwo.Id)
            }));

            var userOneClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userTwo.UserName),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));

            productControllerUserOne = new ProductController(productService);
            productControllerUserTwo = new ProductController(productService);

            productControllerUserTwo.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userTwoClaims }
            };

            productControllerUserOne.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userOneClaims }
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.SaveChangesAsync();
        }
    }
}
