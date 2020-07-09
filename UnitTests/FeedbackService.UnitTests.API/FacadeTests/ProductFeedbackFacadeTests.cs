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
    public class ProductFeedbackFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public ProductFeedbackFacadeTests(DataFixture dataFixture)
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
            long productId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();

            // orderId and customerId won't match and will throw an exception
            customer.Sid = 1;
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.CreateAsync(customerId, orderId, productId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (User 0 does not own an order with Id 0)", ex.Message);
        }

        [Fact]
        public void CreateAsync_FeedbackAlreadyExists_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            order.FeedbackSid = feedback.Sid;

            var cachedFeedbackList = new List<Feedback> { feedback };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.CreateAsync(customerId, orderId, productId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (The product you are trying to rate has already been rated. Try modifying its feedback instead.)", ex.Message);
        }

        [Fact]
        public void CreateAsync_ValidateRating_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var order = _dataFixture.GetOrder(now);
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            feedback.Rating = 6;
            order.FeedbackSid = feedback.Sid;

            var cachedFeedbackList = new List<Feedback> { };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.CreateAsync(customerId, orderId, productId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Invalid rating. Rating must be between 1 to 5.)", ex.Message);
        }

        [Fact]
        public void GetAsync_GetFromCache_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { feedback };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = productFeedbackFacade.GetAsync(customerId, orderId, productId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(2, result.FeedbackType);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal("This is a comment", result.Comment);
        }

        [Fact]
        public void GetAsync_OrderHasNoProducts_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { };
            OrderToProduct orderToProduct = null;

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());
            mockRepository
                .Setup(repo => repo.GetAsync<OrderToProduct>(otp => otp.Ordersid == orderId && otp.ProductSid == productId, CancellationToken.None))
                .ReturnsAsync(orderToProduct);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.GetAsync(customerId, orderId, productId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Unnable to retieve products associated to orderId 0)", ex.Message);
        }

        [Fact]
        public void GetAsync_ProductHasNotBeenRated_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { };
            OrderToProduct orderToProduct = new OrderToProduct
            {
                Ordersid = orderId,
                ProductSid = productId,
                Ammount = 1,
                FeedbackSid = null
            };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());
            mockRepository
                .Setup(repo => repo.GetAsync<OrderToProduct>(otp => otp.Ordersid == orderId && otp.ProductSid == productId, CancellationToken.None))
                .ReturnsAsync(orderToProduct);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.GetAsync(customerId, orderId, productId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (The product has not been rated)", ex.Message);
        }

        [Fact]
        public void GetAsync_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            feedback.Products = new List<Product>();
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { };
            var product = _dataFixture.GetProduct();
            OrderToProduct orderToProduct = new OrderToProduct
            {
                Ordersid = orderId,
                ProductSid = productId,
                Ammount = 1,
                FeedbackSid = 0
            };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository
                .Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>())
                .Returns(() => new ProductFeedbackManager());

            mockRepository
                .Setup(repo => repo.GetAsync<OrderToProduct>(otp => otp.Ordersid == orderId && otp.ProductSid == productId, CancellationToken.None))
                .ReturnsAsync(orderToProduct);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(p => p.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            mockRepository
                .Setup(repo => repo.GetAsync<Feedback>(feedback => feedback.Sid == orderToProduct.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Product, CancellationToken.None))
                .ReturnsAsync(feedback);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = productFeedbackFacade.GetAsync(customerId, orderId, productId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(2, result.FeedbackType);
            Assert.Equal(0, result.CustomerSid);
            Assert.Equal("This is a comment", result.Comment);
        }

        [Fact]
        public void UpdateAsync_ValidateRating_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            feedback.Rating = 6;

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository.Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>()).Returns(() => new ProductFeedbackManager());

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFeedbackFacade.UpdateAsync(customerId, orderId, productId, feedback, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Invalid rating. Rating must be between 1 to 5.)", ex.Message);
        }

        [Fact]
        public void UpdateAsync_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            feedback.Products = new List<Product>();
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { };
            var product = _dataFixture.GetProduct();
            OrderToProduct orderToProduct = new OrderToProduct
            {
                Ordersid = orderId,
                ProductSid = productId,
                Ammount = 1,
                FeedbackSid = 0
            };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository
                .Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>())
                .Returns(() => new ProductFeedbackManager());

            mockRepository
                .Setup(repo => repo.GetAsync<OrderToProduct>(otp => otp.Ordersid == orderId && otp.ProductSid == productId, CancellationToken.None))
                .ReturnsAsync(orderToProduct);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(p => p.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            mockRepository
                .Setup(repo => repo.GetAsync<Feedback>(feedback => feedback.Sid == orderToProduct.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Product, CancellationToken.None))
                .ReturnsAsync(feedback);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            feedback.Comment = "Updated";
            feedback.Rating = 5;

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act
            var result = productFeedbackFacade.UpdateAsync(customerId, orderId, productId, feedback, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Comment);
            Assert.Equal(5, result.Rating);
        }

        [Fact]
        public void DeleteAsync_OrderBelongsToCustomer_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var order = _dataFixture.GetOrder(now);
            order.CustomerSid = 1;
            var product = _dataFixture.GetProduct();

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockRepository
                .Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>())
                .Returns(() => new ProductFeedbackManager());

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(p => p.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AggregateException>(() => productFeedbackFacade.DeleteAsync(customerId, orderId, productId, CancellationToken.None));
        }

        [Fact]
        public void DeleteAsync_ProductHasNotBeenRated_Test()
        {
            // Arrange
            var now = DateTime.Now;
            long orderId = 0;
            long customerId = 0;
            long productId = 0;
            var customer = _dataFixture.GetCustomer();
            var feedback = _dataFixture.GetFeedback(now, FeedbackType.Product);
            feedback.Products = new List<Product>();
            var order = _dataFixture.GetOrder(now);
            order.FeedbackSid = feedback.Sid;
            var cachedFeedbackList = new List<Feedback> { };
            var product = _dataFixture.GetProduct();
            OrderToProduct orderToProduct = new OrderToProduct
            {
                Ordersid = orderId,
                ProductSid = productId,
                Ammount = 1,
                FeedbackSid = null
            };

            _dataFixture.GetMocks<Feedback>(out var mockRepository, out var mockCacheManager, out var mockOptions);
            _dataFixture.GetMocks(out var orderFacadeMock, out var customerFacadeMock, out var productFacadeMock);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Feedback>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedFeedbackList);

            mockRepository
                .Setup(repo => repo.GetManagerInstance<ProductFeedbackManager>())
                .Returns(() => new ProductFeedbackManager());

            mockRepository
                .Setup(repo => repo.GetAsync<OrderToProduct>(otp => otp.Ordersid == orderId && otp.ProductSid == productId, CancellationToken.None))
                .ReturnsAsync(orderToProduct);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(p => p.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            mockRepository
                .Setup(repo => repo.GetAsync<Feedback>(feedback => feedback.Sid == orderToProduct.FeedbackSid && feedback.FeedbackType == (int)FeedbackType.Product, CancellationToken.None))
                .ReturnsAsync((Feedback)null);

            customerFacadeMock
                .Setup(facade => facade.GetCustommerByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            orderFacadeMock
                .Setup(facade => facade.GetOrderByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            var productFeedbackFacade = new ProductFeedbackFacade
                (mockRepository.Object,
                orderFacadeMock.Object,
                customerFacadeMock.Object,
                productFacadeMock.Object,
                mockCacheManager.Object,
                mockOptions.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<AggregateException>(() => productFeedbackFacade.DeleteAsync(customerId, orderId, productId, CancellationToken.None));
        }
    }
}
