using FeedbackService.DataAccess.Models;
using FeedbackService.Enums;
using FeedbackService.Facade;
using FeedbackService.Managers;
using FeedbackService.UnitTests.Fixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.FacadeTests
{
    public class OrderFeedbackFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public OrderFeedbackFacadeTests(DataFixture dataFixture)
        {
            _dataFixture = dataFixture;
        }

        [Fact]
        public void CreateAsync_ValidateOrderDoesNotBelongToCustomer_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();

            // orderId and customerId won't match and will throw an exception
            customer.Sid = 1;
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object, 
                mockCacheManager.Object, 
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.CreateAsync(customerId, orderId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (User 0 does not own an order with Id 0)", ex.Message);
        }

        [Fact]
        public void CreateAsync_FeedbackAlreadyExists_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);
            order.FeedbackSid = feedback.Sid;

            var cachedFeedbackList = new List<Feedback> { feedback };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.CreateAsync(customerId, orderId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (The order you are trying to rate has already been rated. Try modifying its feedback instead.)", ex.Message);
        }

        [Fact]
        public void CreateAsync_ValidateRating_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);

            //This is an invalid rating and should throw an exception
            feedback.Rating = 20;
            var cachedFeedbackList = new List<Feedback>();

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.CreateAsync(customerId, orderId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Invalid rating. Rating must be between 1 to 5.)", ex.Message);
        }

        [Fact]
        public void GetAsync_GetFromCache_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { feedback };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = orderFeedbackFacade.GetAsync(customerId, orderId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(1, result.FeedbackType);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal("This is a comment", result.Comment);
        }

        [Fact]
        public void GetAsync_OrderHasNotBeenRated_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var customer = _dataFixture.GetCustomer();
            var order = _dataFixture.GetOrder(now);
            var cachedFeedbackList = new List<Feedback> { };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.GetAsync(customerId, orderId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Order with Id 0 has not been rated. There is no feedback to retrieve.)", ex.Message);
        }

        [Fact]
        public void GetLatestAsync_ValidateRating_Test()
        {
            // Arrange
            int? rating = 6;
            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.GetLatestAsync(rating, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Invalid rating. Rating must be between 1 to 5.)", ex.Message);
        }

        [Fact]
        public void GetLatestAsync_NullRating_Test()
        {
            // Arrange
            int? rating = null;
            var now = DateTime.Now;

            var cachedFeedbackList = _dataFixture.GetFeedbackList();

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = orderFeedbackFacade.GetLatestAsync(rating, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20, result.Count);
        }

        [Fact]
        public void GetLatestAsync_RatingNotNull_Test()
        {
            // Arrange
            int? rating = 1;
            var now = DateTime.Now;

            var cachedFeedbackList = _dataFixture.GetFeedbackList();

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = orderFeedbackFacade.GetLatestAsync(rating, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Count);
        }

        [Fact]
        public void UpdateAsync_ValidateRating_Test()
        {
            // Arrange
            long customerId = 0;
            long orderId = 0;
            var now = DateTime.Now;
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);
            feedback.Rating = 6;

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFeedbackFacade.UpdateAsync(customerId, orderId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Invalid rating. Rating must be between 1 to 5.)", ex.Message);
        }

        [Fact]
        public void UpdateAsync_Test()
        {
            // Arrange
            long customerId = 0;
            long orderId = 0;
            var now = DateTime.Now;
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Order);
            var cachedFeedbackList = new List<Feedback> { feedback };
            var customer = _dataFixture.GetCustomer();
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());
            mockRepository.Setup(repo => repo.Update<Feedback>(It.IsAny<Feedback>())).Verifiable();

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = orderFeedbackFacade.UpdateAsync(customerId, orderId, feedback, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(1, result.FeedbackType);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal("This is a comment", result.Comment);
        }

        [Fact]
        public void DeleteAsync_ValidateOrderDoesNotBelongToCustomer_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();

            // orderId and customerId won't match and will throw an exception
            customer.Sid = 1;

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<OrderFeedbackManager>()).Returns(() => new OrderFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var orderFeedbackFacade = new OrderFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AggregateException>(() => orderFeedbackFacade.DeleteAsync(customerId, orderId, CancellationToken.None));
        }
    }
}
