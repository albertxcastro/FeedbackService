using FeedbackService.DataAccess.Models;
using FeedbackService.Facade;
using FeedbackService.UnitTests.Fixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace FeedbackService.UnitTests.FacadeTests
{
    public class CustomerFacadeTests : IClassFixture<DataFixture>
    {
        private readonly DataFixture _dataFixture;

        public CustomerFacadeTests(DataFixture dataFixture)
        {
            _dataFixture = dataFixture;
        }

        [Fact]
        public void GetCustommerByIdAsync_GetFromDb_Test()
        {
            // Arrange
            Customer customer = _dataFixture.GetCustomer();
            long customerId = 0;
            List<Customer> cachedCustomerList = null;
            _dataFixture.GetMocks<Customer>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Customer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedCustomerList);

            mockRepository
                .Setup(repo => repo.GetAsync<Customer>(customer => customer.Sid == customerId, CancellationToken.None))
                .ReturnsAsync(customer);

            var customerFacade = new CustomerFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetCustommerByIdAsync(customerId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alberto", result.FirstName);
            Assert.Equal("Castro", result.LastName);
            Assert.Equal("password", result.Password);
            Assert.Equal("admin", result.Username);
            Assert.Equal(0, result.Sid);
        }

        [Fact]
        public void GetCustommerByIdAsync_GetFromCache_Test()
        {
            // Arrange
            Customer customer = _dataFixture.GetCustomer();
            long customerId = 0;
            List<Customer> cachedCustomerList = new List<Customer> { customer };
            _dataFixture.GetMocks<Customer>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Customer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedCustomerList);

            mockRepository
                .Setup(repo => repo.GetAsync<Customer>(customer => customer.Sid == customerId, CancellationToken.None))
                .ReturnsAsync((Customer)null);

            var customerFacade = new CustomerFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act
            var result = customerFacade.GetCustommerByIdAsync(customerId, CancellationToken.None).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alberto", result.FirstName);
            Assert.Equal("Castro", result.LastName);
            Assert.Equal("password", result.Password);
            Assert.Equal("admin", result.Username);
            Assert.Equal(0, result.Sid);
        }

        [Fact]
        public void GetCustommerByIdAsync_UserDoesNotExistInCacheNorInDb_Test()
        {
            // Arrange
            Customer customer = null;
            long customerId = 0;
            List<Customer> cachedCustomerList = null;
            _dataFixture.GetMocks<Customer>(out var mockRepository, out var mockCacheManager, out var mockOptions);

            mockCacheManager
                .Setup(cache => cache.GetFromCacheAsync<List<Customer>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedCustomerList);

            mockRepository
                .Setup(repo => repo.GetAsync<Customer>(customer => customer.Sid == customerId, CancellationToken.None))
                .ReturnsAsync(customer);

            var customerFacade = new CustomerFacade(mockRepository.Object, mockCacheManager.Object, mockOptions.Object);

            // Act & Assert
            var ex = Assert.Throws<AggregateException>(() => customerFacade.GetCustommerByIdAsync(customerId, CancellationToken.None).Result);
            Assert.Equal("One or more errors occurred. (Unnable to retieve customer with orderId 0)", ex.Message);
        }
    }
}
