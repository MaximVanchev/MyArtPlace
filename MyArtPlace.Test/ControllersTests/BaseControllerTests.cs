using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using MyArtPlace.Controllers;
using MyArtPlace.Core.Constants;
using MyArtPlace.Core.Contracts;
using MyArtPlace.Core.Models.Common;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyArtPlace.Test.ControllersTests
{
    public class BaseControllerTests
    {
        private Mock<IProductService> productService;
        private BaseController baseController;
        private HomeController homeController;

        [SetUp]
        public async Task Setup()
        {
            productService = new Mock<IProductService>();
            baseController = new BaseController();
            homeController = new HomeController(productService.Object);
        }

        [Test]
        public void WhenAddSuccessMessageToMessageViewModelShouldBeAddedToViewData()
        {
            string message = "test";
            MessageViewModel.Message.Add(MessageConstants.SuccessMessage, message);

            var result = homeController.Test().Result as ViewResult;
            ViewDataDictionary viewData = result.ViewData;
            Assert.AreEqual(message, viewData[MessageConstants.SuccessMessage]);
        }

        [Test]
        public void WhenAddErrorMessageToMessageViewModelShouldBeAddedToViewData()
        {
            string message = "test";
            MessageViewModel.Message.Add(MessageConstants.ErrorMessage, message);

            var result = homeController.Test().Result as ViewResult;
            ViewDataDictionary viewData = result.ViewData;
            Assert.AreEqual(message, viewData[MessageConstants.ErrorMessage]);
        }

        [Test]
        public void WhenAddWarningMessageToMessageViewModelShouldBeAddedToViewData()
        {
            string message = "test";
            MessageViewModel.Message.Add(MessageConstants.WarningMessage, message);

            var result = homeController.Test().Result as ViewResult;
            ViewDataDictionary viewData = result.ViewData;
            Assert.AreEqual(message, viewData[MessageConstants.WarningMessage]);
        }

        [TearDown]
        public void TearDown()
        { }
    }
}
