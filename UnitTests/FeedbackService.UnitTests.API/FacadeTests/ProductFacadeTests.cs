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
    public class ProductFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public ProductFacadeTests(DataFixture dataFixture)
        {
            _dataFixture = dataFixture;
        }

        [Fact]
        public void GetProductByIdAsync_GetFromDb_Test()
        {
            // Arrange
            var product = _dataFixture.GetProduct();
            long productId = 0;
            List<Product> cachedProductList = null;

            _dataFixture.GetMocks<Product>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Product>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedProductList);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(product => product.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            var customerFacade = new ProductFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetProductByIdAsync(productId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(20, result.Price);
            Assert.Equal("Coca cola", result.Name);
        }

        [Fact]
        public void GetProductByIdAsync_GetFromCache_Test()
        {
            // Arrange
            var product = _dataFixture.GetProduct();
            long productId = 0;
            List<Product> cachedProductList = new List<Product> { product };

            _dataFixture.GetMocks<Product>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Product>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedProductList);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(product => product.Sid == productId, CancellationToken.None))
                .ReturnsAsync((Product)null);

            var customerFacade = new ProductFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetProductByIdAsync(productId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Sid);
            Assert.Equal(20, result.Price);
            Assert.Equal("Coca cola", result.Name);
        }

        [Fact]
        public void GetProductByIdAsync_ProductDoesNotExistInCacheNorInDb_Test()
        {
            // Arrange
            Product product = null;
            long productId = 0;
            List<Product> cachedProductList = null;
            _dataFixture.GetMocks<Product>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Product>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedProductList);

            mockRepository
                .Setup(repo => repo.GetAsync<Product>(product => product.Sid == productId, CancellationToken.None))
                .ReturnsAsync(product);

            var productFacade = new ProductFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => productFacade.GetProductByIdAsync(productId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Unnable to retieve product with id 0)", ex.Message);
        }
    }
}
