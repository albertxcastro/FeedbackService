using FeedbackService.Controllers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.API.ControllerTests
{
    public class ProductFeedbackControllerTests : ControllerTestBase
    {
        [Fact]
        public void GetAsync_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetAsync_NoUserID_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult.Result);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void PostAsync_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.CreateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void PostAsync_NoUserID_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.CreateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult.Result);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void PutAsync_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.UpdateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void PutAsync_NoUserID_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.UpdateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetAsync(orderId, productId, CancellationToken.None);

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult.Result);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void DeleteAsync_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.DeleteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).Verifiable();

            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.DeleteAsync(orderId, productId, CancellationToken.None);

            // Assert
            var val = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal("Correctly deleted", val.Value);
        }

        [Fact]
        public void DeleteAsync_NoUserId_Test()
        {
            // Arrange
            long orderId = 0;
            long productId = 0;

            var mockFacade = new Mock<IProductFeedbackFacade>();
            mockFacade.Setup(facade => facade.DeleteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).Verifiable();

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.DeleteAsync(orderId, productId, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        private ProductFeedbackController GetControllerInstance(IProductFeedbackFacade facadeObject, Dictionary<string, string> headers = default)
        {
            var context = GetHttpContext(headers);
            var controllerContext = new ControllerContext { HttpContext = context };
            return new ProductFeedbackController(facadeObject) { ControllerContext = controllerContext };
        }
    }
}
