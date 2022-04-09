using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Product;
using MyArtPlace.Core.Services;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Test.ServicesTests
{
    public class ProductServiceTests
    {
        private InMemoryDbContext dbContext;
        private ServiceProvider serviceProvider;
        private MyArtPlaceUser userOne;
        private MyArtPlaceUser userTwo;
        private MyArtPlaceUser userTree;

        [SetUp]
        public async Task SetUp()
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
        }

        [Test]
        public void WhenUserAddProductShouldBeAdded()
        {
            var service = serviceProvider.GetService<IProductService>();
            var model = new ProductViewModel()
            {
                Category = "BGN",
                Price = 30,
                Name = "Kotka"
            };
            string defaultImage = @"./../../../../MyArtPlace/wwwroot/corona_bootstrap/images/default_product_image.png";

            using (var stream = File.OpenRead(defaultImage))
            {
                model.Image = new FormFile(stream, 0, stream.Length, "image", Path.GetFileName(stream.Name));
                using (var memoryStream = new MemoryStream())
                {
                    model.Image.CopyToAsync(memoryStream);

                    model.ImageByteArray = memoryStream.ToArray();
                }
            }

            service.AddProduct(model, userTwo.Id);

            Assert.True(userTwo.Shop.Products.Count() == 1);
        }

        [Test]
        public void WhenUserAddProductWithNoImageMustThrow()
        {
            var service = serviceProvider.GetService<IProductService>();
            var model = new ProductViewModel()
            {
                Category = "BGN",
                Price = 30,
                Name = "Kotka"
            };

            var ex = Assert.CatchAsync<ArgumentException>( async () => await service.AddProduct(model, userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.ImageIsNullErrorMessage);
        }

        [Test]
        public void WhenGetAllCategoriesShouldReturnThemCorrect()
        {
            var service = serviceProvider.GetService<IProductService>();
            var allCategories = new List<Category>();

            foreach (var product in userOne.Shop.Products)
            {
                allCategories.Add(product.Category);
            }

            allCategories.Should().BeEquivalentTo(service.GetAllCategories().Result.ToList());
        }

        [Test]
        public void WhenUserEditProductShouldBeEdited()
        {
            string name = "Kuche";
            string description = "kuche";
            string category = "Prints";
            decimal price = 30;
            var service = serviceProvider.GetService<IProductService>();
            var model = new ProductEditViewModel()
            {
                Id = userOne.Shop.Products.First().Id,
                Name = name,
                Description = description,
                Category = category,
                Price = price
            };

            service.EditProduct(model);

            Assert.True(userOne.Shop.Products.First().Name == name);
            Assert.True(userOne.Shop.Products.First().Description == description);
            Assert.True(userOne.Shop.Products.First().Category.Name == category);
            Assert.True(userOne.Shop.Products.First().Price == price);
        }

        [Test]
        public void WhenyGetCategoryByNameShouldReturnCategory()
        {
            string categoryName = "Prints";
            var service = serviceProvider.GetService<IProductService>();

            Assert.AreEqual(categoryName, service.GetCategoryByName(categoryName).Result.Name);
        }

        [Test]
        public void WhenGetProductForEditShouldReturnProduct()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userOne.Shop.Products.First();
            var model = new ProductEditViewModel()
            {
                Name = product.Name,
                Category = product.Category.Name,
                Description = product.Description,
                Price = product.Price,
                Id = product.Id,
                ImageByteArray = product.Image,
                AllCategories = service.GetAllCategories().Result
            };

            model.Should().BeEquivalentTo(service.GetProductForEdit(product.Id, userOne.Id).Result);
        }

        [Test]
        public void WhenUserGetProductForEditThatIsNotHisMustThrow()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userOne.Shop.Products.First();

            var ex = Assert.CatchAsync<ArgumentException>(async () => await service.GetProductForEdit(product.Id, userTwo.Id));
            Assert.AreEqual(ex.Message, MessageConstants.UserDontHaveProductForEditErrorMessage);
        }

        [Test]
        public void WhenGetUserProductsShouldReturnUserProducts()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userOne.Shop.Products.First();
            var model = new ProductListViewModel
            {
                Name = product.Name,
                Category = product.Category.Name,
                Id = product.Id,
                ImageByteArray = product.Image,
                Price = product.Price,
                Iso = product.Shop.Currency.Iso,
                Likes = product.UsersLiked.Count()
            };

            Assert.True(service.GetUserProducts(userOne.Id).Result.Count() == userOne.Shop.Products.Count);
            model.Should().BeEquivalentTo(service.GetUserProducts(userOne.Id).Result.First(x => x.Id == product.Id));
        }

        [Test]
        public void WhenGetAllProductsShouldReturnAllProducts()
        {
            int allProductCount = 3;
            var service = serviceProvider.GetService<IProductService>();
            var product = userOne.Shop.Products.First();
            var model = new ProductListViewModel
            {
                Name = product.Name,
                Category = product.Category.Name,
                Id = product.Id,
                ImageByteArray = product.Image,
                Likes = product.UsersLiked.Count(),
                Price = product.Price,
                Iso = product.Shop.Currency.Iso,
                UserLiked = product.UsersLiked.Contains(userOne)
            };

            Assert.True(service.GetAllProducts(userOne.Id).Result.Count() == allProductCount);
            model.Should().BeEquivalentTo(service.GetAllProducts(userOne.Id).Result.First(x => x.Id == product.Id));
        }

        [Test]
        public void WhenUserDeleteProductShouldBeDeleted()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            service.DeleteProduct(product.Id , userTree.Id);

            Assert.True(userTree.Shop.Products.Count() == 0);
        }

        [Test]
        public void WhenUserDeleteProductThatDontHaveMustThrow()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            var ex = Assert.CatchAsync(async () => await service.DeleteProduct(product.Id, userOne.Id));
            Assert.AreEqual(ex.Message, MessageConstants.DontHavePermissionToDelete);
        }

        [Test]
        public void WhenUserLikeProductShouldBeLiked()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            service.LikeProduct(product.Id, userOne.Id);

            Assert.True(userOne.LikedProducts.Count() == 1);
            product.Should().BeEquivalentTo(userOne.LikedProducts.First());
        }

        [Test]
        public void WhenUserLikeProductThatHaveMustThrow()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            var ex = Assert.CatchAsync(async () => await service.LikeProduct(product.Id, userTree.Id));
            Assert.AreEqual(ex.Message, MessageConstants.CantLikeYourProductErrorMessage);
        }

        [Test]
        public void WhenUserDislikeProductShouldBeDisliked()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            service.LikeProduct(product.Id, userOne.Id);
            service.DislikeProduct(product.Id, userOne.Id);

            Assert.True(userOne.LikedProducts.Count() == 0);
        }

        [Test]
        public void WhenUserDislikeProductThatHaveMustThrow()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();

            var ex = Assert.CatchAsync(async () => await service.DislikeProduct(product.Id, userTree.Id));
            Assert.AreEqual(ex.Message, MessageConstants.CantDislikeYourProductErrorMessage);
        }

        [Test]
        public void WhenGetUserFavoritesProductsShouldReturnUserFavoritesProducts()
        {
            var service = serviceProvider.GetService<IProductService>();
            var productToLike = userTree.Shop.Products.First();

            service.LikeProduct(productToLike.Id, userOne.Id);

            var product = userOne.LikedProducts.First();
            var model = new ProductListViewModel
            {
                Name = product.Name,
                Category = product.Category.Name,
                Id = product.Id,
                ImageByteArray = product.Image,
                Price = product.Price,
                Iso = product.Shop.Currency.Iso,
                Likes = product.UsersLiked.Count()
            };

            Assert.True(service.GetUserFavoritesProducts(userOne.Id).Result.Count() == userOne.LikedProducts.Count());
            model.Should().BeEquivalentTo(service.GetUserFavoritesProducts(userOne.Id).Result.First(x => x.Id == productToLike.Id));
        }

        [Test]
        public void WhenGetProductDetailsShouldReturnProductDetails()
        {
            var service = serviceProvider.GetService<IProductService>();
            var product = userTree.Shop.Products.First();
            var model = new ProductDetailsViewModel()
            {
                Name = product.Name,
                Category = product.Category.Name,
                Description = product.Description,
                Price = product.Price,
                ImageByteArray = product.Image,
                Currency = product.Shop.Currency.Iso
            };

            model.Should().BeEquivalentTo(service.GetProductDetails(product.Id).Result);
        }

        [TearDown]
        public void TearDown()
        {
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
                UserName = "Pesho",
                Email = "maxi.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "PeshoShop",
                    Currency = new Currency()
                    {
                        Iso = "USD"
                    }
                }
            };

            userTree = new MyArtPlaceUser()
            {
                UserName = "Ivo",
                Email = "max.van.mv@gmail.com",
                PasswordHash = "1234",
                EmailConfirmed = true,
                Shop = new Shop()
                {
                    Name = "IvoShop",
                    Currency = userOne.Shop.Currency,
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            Name = "Prase",
                            Category = userOne.Shop.Products.First().Category,
                            Image = new byte[20],
                            Price = 30
                        }
                    }
                }
            };

            await repo.AddAsync(userOne);
            await repo.AddAsync(userTwo);
            await repo.AddAsync(userTree);
            await repo.SaveChangesAsync();
        }
    }
}
