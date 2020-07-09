using FeedbackService.DataAccess.Models;
using FeedbackService.Facade;
using FeedbackService.UnitTests.Fixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.FacadeTests
{
    public class OrderFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public OrderFacadeTests(DataFixture dataFixture)
        {
            _dataFixture = dataFixture;
        }

        [Fact]
        public void GetOrderByIdAsync_GetFromDb_Test()
        {
            // Arrange
            var createTime = DateTime.Now;
            Order order = _dataFixture.GetOrder(createTime);
            long orderId = 0;
            List<Order> cachedOrderList = null;
            _dataFixture.GetMocks<Order>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Order>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedOrderList);

            mockRepository
                .Setup(repo => repo.GetAsync<Order>(order => order.Sid == orderId, CancellationToken.None))
                .ReturnsAsync(order);

            var customerFacade = new OrderFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetOrderByIdAsync(orderId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Products);
            Assert.Null(result.FeedbackSid);
            Assert.Equal(createTime, result.CreateTime);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal(0, result.Sid);
        }

        [Fact]
        public void GetOrderByIdAsync_GetFromCache_Test()
        {
            // Arrange
            var createTime = DateTime.Now;
            Order order = _dataFixture.GetOrder(createTime);
            long orderId = 0;
            List<Order> cachedOrderList = new List<Order> { order };
            _dataFixture.GetMocks<Order>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Order>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedOrderList);

            mockRepository
                .Setup(repo => repo.GetAsync<Order>(order => order.Sid == orderId, CancellationToken.None))
                .ReturnsAsync((Order)null);

            var customerFacade = new OrderFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetOrderByIdAsync(orderId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Products);
            Assert.Null(result.FeedbackSid);
            Assert.Equal(createTime, result.CreateTime);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal(0, result.Sid);
        }

        [Fact]
        public void GetOrderByIdAsync_OrderDoesNotExistInCacheNorInDb_Test()
        {
            // Arrange
            Order order = null;
            long orderId = 0;
            List<Order> cachedOrderList = null;
            _dataFixture.GetMocks<Order>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Order>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedOrderList);

            mockRepository
                .Setup(repo => repo.GetAsync<Order>(order => order.Sid == orderId, CancellationToken.None))
                .ReturnsAsync(order);

            var orderFacade = new OrderFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => orderFacade.GetOrderByIdAsync(orderId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Unnable to retieve order with orderId 0)", ex.Message);
        }
    }
}
