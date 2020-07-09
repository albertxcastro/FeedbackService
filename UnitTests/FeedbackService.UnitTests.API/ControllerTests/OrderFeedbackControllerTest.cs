using FeedbackService.Controllers;
using FeedbackService.DataAccess.Models;
using FeedbackService.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.API.ControllerTests
{
    public class OrderFeedbackControllerTest : ControllerTestBase
    {
        [Fact]
        public void PostAsync_Test()
        {
            // Arrange
            long orderId = 0;
            var feedback = new Feedback { Comment = "New Comment", Rating = 5 };
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.CreateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);
            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.PostAsync(orderId, feedback, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void PostAsync_NoUserId_Test()
        {
            // Arrange
            long orderId = 0;
            Feedback feedback = default;
            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.CreateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.PostAsync(orderId, feedback, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void GetAsync_Test()
        {
            // Arrange
            long orderId = 0;
            Feedback feedback = new Feedback();
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);

            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.GetAsync(orderId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetAsync_NoUserId_Test()
        {
            // Arrange
            long orderId = 0;
            Feedback feedback = default;

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetAsync(orderId, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void GetLatestAsync_Test()
        {
            // Arrange
            int? rating = null;
            var feedbackList = new List<Feedback> { new Feedback() };
            
            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetLatestAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedbackList);

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var okResult = controller.GetLatestAsync(rating, CancellationToken.None).Result;

            // Assert
            var val = Assert.IsType<OkObjectResult>(okResult);
            Assert.NotNull(val.Value);
        }

        [Fact]
        public void GetLatestAsync_ThrowException_Test()
        {
            // Arrange
            int? rating = null;
            var feedbackList = new List<Feedback> { new Feedback() };

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.GetLatestAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>())).Throws(new Exception("ArgumentNullException message"));

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetLatestAsync(rating, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult);
            Assert.Equal("ArgumentNullException message", result.Content);
        }

        [Fact]
        public void PutAsync_Test()
        {
            // Arrange
            long orderId = 0;
            Feedback feedback = default;
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.UpdateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);

            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.GetAsync(orderId, CancellationToken.None);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void PutAsync_NoUserId_Test()
        {
            // Arrange
            long orderId = 0;
            Feedback feedback = default;

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.UpdateAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<Feedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(feedback);

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.GetAsync(orderId, CancellationToken.None);

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult.Result);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        [Fact]
        public void DeleteAsync_Test()
        {
            // Arrange
            long orderId = 0;
            var header = new Dictionary<string, string>
            {
                { "UserId", "1" }
            };

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.DeleteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).Verifiable();

            var controller = GetControllerInstance(mockFacade.Object, header);

            // Act
            var okResult = controller.DeleteAsync(orderId, CancellationToken.None);

            // Assert
            var val = Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.Equal("Correctly Deleted", val.Value);
        }

        [Fact]
        public void DeleteAsync_NoUserId_Test()
        {
            // Arrange
            long orderId = 0;

            var mockFacade = new Mock<IOrderFeedbackFacade>();
            mockFacade.Setup(facade => facade.DeleteAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).Verifiable();

            var controller = GetControllerInstance(mockFacade.Object);

            // Act
            var contentResult = controller.DeleteAsync(orderId, CancellationToken.None).Result;

            // Assert
            var result = Assert.IsType<ContentResult>(contentResult);
            Assert.Equal("UserId was not set in header", result.Content);
        }

        private OrderFeedbackController GetControllerInstance(IOrderFeedbackFacade facadeObject, Dictionary<string, string> headers = default)
        {
            var context = GetHttpContext(headers);
            var controllerContext = new ControllerContext { HttpContext = context };
            return new OrderFeedbackController(facadeObject) { ControllerContext = controllerContext };
        }
    }
}
